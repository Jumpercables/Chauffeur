using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Model;

using Newtonsoft.Json;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     Provides a WCF service for chauffering the builds onto remote machines.
    /// </summary>
    [ServiceContract]
    public interface IChauffeurService
    {
        #region Public Methods

        /// <summary>
        ///     Gets the package that has been installed for the job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>Returns a <see cref="Package" /> representing the installed package.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "Package/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Task<Package> GetPackageAsync(string jobName);

        /// <summary>
        ///     Download and installs the last successful build on the machine that host the service.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> representing the build that was installed on the machine.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Install/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> InstallBuildAsync(string jobName);

        /// <summary>
        ///     Uninstalls the build the was installed for the job on the host machine.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        [OperationContract]
        [WebGet(UriTemplate = "Uninstall/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> UninstallBuildAsync(string jobName);

        #endregion
    }

    /// <summary>
    ///     Provides a WCF service that will install builds on the host machines.
    /// </summary>
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ChauffeurService : JenkinsService, IChauffeurService
    {
        #region IChauffeurService Members

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        public async Task<Build> InstallBuildAsync(string jobName)
        {
            // Uninstall previous packages.
            await this.UninstallBuildAsync(jobName);

            // Query jenkins for the build information.
            var build = await this.GetBuild(jobName);

            // Download the packages.
            var packages = await this.DownloadPackages(build);

            // Install the packages.
            this.InstallPackages(packages);

            // Send the notifications.
            this.Notify(build);

            // Save the last build installed.
            this.AddPackage(jobName, build, packages);

            // Return the build.
            return build;
        }

        /// <summary>
        ///     Uninstalls the build the was installed for the job on the host machine.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        public async Task<Build> UninstallBuildAsync(string jobName)
        {
            var packages = await this.GetPackagesAsync();
            var package = packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            if (package != null)
            {
                this.Log("Uninstalling {0} package(s).", package.Build.Artifacts.Count);

                foreach (var artifact in package.Build.Artifacts)
                {
                    string pkg = Path.Combine(this.Configuration.DataDirectory, artifact.FileName);

                    await this.UninstallPackageAsync(pkg).ContinueWith((task) =>
                    {
                        File.Delete(pkg);

                        if (packages.Contains(package))
                            packages.Remove(package);

                        this.UpdatePackages(packages);
                    });
                }


                return package.Build;
            }

            return null;
        }

        /// <summary>
        ///     Gets the build from chauffuer file for the specified job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build that was installed for the job.
        /// </returns>
        public async Task<Package> GetPackageAsync(string jobName)
        {
            var packages = await this.GetPackagesAsync();

            return packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the build that was installed to the chauffuer file.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="value">The value.</param>
        /// <param name="paths">The paths.</param>
        private async void AddPackage(string jobName, Build value, string[] paths)
        {
            List<Package> packages = await this.GetPackagesAsync();
            var package = packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            if (package == null)
            {
                package = new Package
                {
                    Job = jobName,
                    Build = value,
                    Paths = paths
                };

                packages.Add(package);
            }
            else
            {
                package.Build = value;
                package.Paths = paths;
                package.Date = DateTime.Now.ToShortDateString();
            }

            this.UpdatePackages(packages);
        }

        /// <summary>
        ///     Downloads the packages of the build.
        /// </summary>
        /// <param name="build">The build.</param>
        private async Task<string[]> DownloadPackages(Build build)
        {
            var service = new ArtifactService(base.BaseUri, base.Client, base.Configuration);
            return await service.DownloadArtifactsAsync(build);
        }

        /// <summary>
        ///     Gets the build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last successful build.
        /// </returns>
        private async Task<Build> GetBuild(string jobName)
        {
            var service = new JobService(base.BaseUri, base.Client, base.Configuration);
            return await service.GetLastSuccessfulBuildAsync(jobName);
        }

        /// <summary>
        ///     Gets the all of the packages that have been installed on the machine.
        /// </summary>
        /// <returns>
        ///     Return <see cref="List{Package}" /> representing the installed packages.
        /// </returns>
        private Task<List<Package>> GetPackagesAsync()
        {
            return Task.Run(() =>
            {
                List<Package> packages = null;

                if (File.Exists(base.Configuration.PackagesJsonFile))
                {
                    var json = File.ReadAllText(base.Configuration.PackagesJsonFile);
                    packages = JsonConvert.DeserializeObject<List<Package>>(json);
                }

                return packages ?? new List<Package>();
            });
        }

        /// <summary>
        ///     Installs the package asynchronously.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>
        ///     Returns the <see cref="Task" /> representing the operation.
        /// </returns>
        private Task InstallPackageAsync(string package)
        {
            return Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} \"{1}\" /quiet {2}", "/i", package, this.Configuration.InstallPropertyReferences));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                this.Log(string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

                using (Process process = Process.Start(startInfo))
                    if (process != null) process.WaitForExit();
            });
        }

        /// <summary>
        ///     Installs the build MSI packages using the "msiexec.exe" utility on the windows machine.
        /// </summary>
        /// <param name="packages">The packages.</param>
        private async void InstallPackages(string[] packages)
        {
            this.Log("Installing {0} package(s).", packages.Length);

            foreach (var pkg in packages)
            {
                await this.InstallPackageAsync(pkg);
            }
        }

        /// <summary>
        ///     Sends the notification of the completion.
        /// </summary>
        /// <param name="build">The build.</param>
        private async void Notify(Build build)
        {
            NotificationService service = new NotificationService();
            await service.SendAsync(build).ContinueWith((task) => this.Log("Notification: {0}", task.Result));
        }

        /// <summary>
        ///     Uninstalls the package asynchronously.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns></returns>
        private Task UninstallPackageAsync(string package)
        {
            return Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} \"{1}\" /quiet {2}", "/x", package, this.Configuration.UninstallPropertyReferences));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                this.Log(string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

                using (Process process = Process.Start(startInfo))
                    if (process != null) process.WaitForExit();
            });
        }

        /// <summary>
        ///     Updates the packages.
        /// </summary>
        /// <param name="packages">The packages.</param>
        private void UpdatePackages(List<Package> packages)
        {
            var json = JsonConvert.SerializeObject(packages, Formatting.Indented);
            File.WriteAllText(base.Configuration.PackagesJsonFile, json);
        }

        #endregion
    }

    [DataContract]
    public class Package
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Package" /> class.
        /// </summary>
        public Package()
        {
            this.Date = DateTime.Now.ToShortDateString();
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "build", Order = 4)]
        public Build Build { get; set; }

        [DataMember(Name = "date", Order = 2)]
        public string Date { get; set; }

        [DataMember(Name = "job", Order = 1)]
        public string Job { get; set; }

        [DataMember(Name = "paths", Order = 3)]
        public string[] Paths { get; set; }

        #endregion
    }
}