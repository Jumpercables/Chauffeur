using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
        ///     Installs the build with the number for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Install/{jobName}/{buildNumber}", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> InstallBuildAsync(string jobName, string buildNumber);

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "InstallLastSuccessfulBuild/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> InstallLastSuccessfulBuildAsync(string jobName);

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
        ///     Installs the build with the number for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        public async Task<Build> InstallBuildAsync(string jobName, string buildNumber)
        {
            try
            {
                // Query jenkins for the build information.
                var build = await this.GetBuildAsync(jobName, buildNumber);

                // Download the packages.
                var packages = await this.DownloadPackagesAsync(build);

                // Uninstall previous packages.
                await this.UninstallBuildAsync(jobName);

                // Install the packages.
                this.InstallPackages(packages);

                // Save the last build installed.
                var package = await this.AddPackage(jobName, build, packages);

                // Send the notifications.
                this.Notify(package);

                // Return the build.
                return build;
            }
            catch (Exception e)
            {
                Log.Error(this, e);
            }

            return null;
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
                Log.Info(this, "Uninstalling {0} package(s).", package.Build.Artifacts.Count);

                foreach (var artifact in package.Build.Artifacts)
                {
                    string pkg = Path.Combine(this.Configuration.ArtifactsDirectory, artifact.FileName);

                    await this.UninstallPackageAsync(pkg).ContinueWith((task) =>
                    {
                        File.Delete(pkg);

                        if (packages.Contains(package))
                            packages.Remove(package);

                        this.SavePackages(packages);
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

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        public async Task<Build> InstallLastSuccessfulBuildAsync(string jobName)
        {
            return await this.InstallBuildAsync(jobName, null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the build that was installed to the chauffuer file.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="build">The build.</param>
        /// <param name="paths">The paths.</param>
        /// <returns>Returns a <see cref="Package" /> representigng the new package.</returns>
        private async Task<Package> AddPackage(string jobName, Build build, string[] paths)
        {
            var packages = await this.GetPackagesAsync();

            var package = packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            if (package == null)
            {
                package = new Package
                {
                    Url = new Uri(this.Configuration.PackagesDataFile),
                    Job = jobName,
                    Build = build,
                    Paths = paths,
                    MachineName = Environment.MachineName
                };

                packages.Add(package);
            }
            else
            {
                package.MachineName = Environment.MachineName;
                package.Build = build;
                package.Paths = paths;
                package.Date = DateTime.Now.ToShortDateString();
            }

            this.SavePackages(packages);

            return package;
        }

        /// <summary>
        ///     Downloads the packages of the build.
        /// </summary>
        /// <param name="build">The build.</param>
        private async Task<string[]> DownloadPackagesAsync(Build build)
        {
            var service = new ArtifactService(base.BaseUri, base.Client, base.Configuration);
            return await service.DownloadArtifactsAsync(build);
        }

        /// <summary>
        ///     Gets the build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build.
        /// </returns>
        private async Task<Build> GetBuildAsync(string jobName, string buildNumber = null)
        {
            var service = new JobService(base.BaseUri, base.Client, base.Configuration);

            if (string.IsNullOrEmpty(buildNumber))
            {
                return await service.GetLastSuccessfulBuildAsync(jobName);
            }

            return await service.GetBuildAsync(jobName, buildNumber);
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
                Log.Info(this, "Loading packages data: {0}", base.Configuration.PackagesDataFile);

                List<Package> packages = null;

                if (File.Exists(base.Configuration.PackagesDataFile))
                {
                    var json = File.ReadAllText(base.Configuration.PackagesDataFile);
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

                Log.Info(this, string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

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
            Log.Info(this, "Installing {0} package(s).", packages.Length);

            foreach (var pkg in packages)
            {
                await this.InstallPackageAsync(pkg);
            }
        }

        /// <summary>
        ///     Sends the notification of the completion.
        /// </summary>
        /// <param name="package">The package.</param>
        private async void Notify(Package package)
        {
            NotificationService service = new NotificationService();
            await service.SendAsync(package).ContinueWith((task) =>
            {
                Log.Info(this, "Notification: {0}", task.Result);
            });
        }

        /// <summary>
        ///     Saves the packages.
        /// </summary>
        /// <param name="packages">The packages.</param>
        private void SavePackages(List<Package> packages)
        {
            var json = JsonConvert.SerializeObject(packages, Formatting.Indented);
            File.WriteAllText(base.Configuration.PackagesDataFile, json);
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

                Log.Info(this, string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

                using (Process process = Process.Start(startInfo))
                    if (process != null) process.WaitForExit();
            });
        }

        #endregion
    }
}