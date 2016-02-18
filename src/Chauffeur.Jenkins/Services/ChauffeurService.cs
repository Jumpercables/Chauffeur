using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Configuration;
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
        ///     Gets the configurations.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="ChauffeurConfiguration" /> representing the configuratons.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Configurations", ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, string> GetConfigurations();

        /// <summary>
        ///     Gets the package that has been installed for the job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>Returns a <see cref="Package" /> representing the installed package.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "Package/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Package GetPackage(string jobName);

        /// <summary>
        ///     Gets the package that has been installed for the job.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="Package" /> representing the installed package.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Packages", ResponseFormat = WebMessageFormat.Json)]
        List<Package> GetPackages();

        /// <summary>
        ///     Gets the URI templates that are availabe for the service.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the methods available.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "?", ResponseFormat = WebMessageFormat.Xml)]
        List<string> GetUriTemplates();

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
        ///     Uninstalls the build the was installed for the job on the host machine.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the build was uninstalled.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Uninstall/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Task<bool> UninstallBuildAsync(string jobName);

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
                var paths = await this.DownloadPackagesAsync(build);

                // Uninstall old packages.
                await this.UninstallBuildAsync(jobName);

                // Install the packages.
                this.Install(paths);

                // Save the last build installed.
                var package = await this.AddPackage(jobName, build, paths);

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
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the build was uninstalled.
        /// </returns>
        public async Task<bool> UninstallBuildAsync(string jobName)
        {
            try
            {
                Log.Info(this, "Uninstalling {0} package(s).", jobName);

                var packages = this.GetPackages();
                var package = packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
                if (package != null)
                {
                    string[] paths = package.Paths;
                    if (paths.Any(o => !File.Exists(o)))
                    {
                        var build = await this.GetBuildAsync(package.Job, package.BuildNumber.ToString(CultureInfo.InvariantCulture));
                        paths = await this.DownloadPackagesAsync(build);
                    }

                    this.Serialize(packages.Where(pkg => pkg != package));
                    this.Uninstall(paths);

                    return true;
                }

                string path = await this.GetPackageCacheAsync();
                if (path != null)
                {
                    this.Uninstall(path);
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(this, e);
            }

            return false;
        }

        /// <summary>
        ///     Gets the build from chauffuer file for the specified job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build that was installed for the job.
        /// </returns>
        public Package GetPackage(string jobName)
        {
            try
            {
                var packages = this.GetPackages();
                return packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                Log.Error(this, e);
            }

            return null;
        }

        /// <summary>
        ///     Gets the all of the packages that have been installed on the machine.
        /// </summary>
        /// <returns>
        ///     Return <see cref="List{Package}" /> representing the installed packages.
        /// </returns>
        public List<Package> GetPackages()
        {
            List<Package> packages = null;

            if (File.Exists(base.Configuration.PackagesDataFile))
            {
                var json = File.ReadAllText(base.Configuration.PackagesDataFile);
                packages = JsonConvert.DeserializeObject<List<Package>>(json);
            }

            return packages ?? new List<Package>();
        }

        /// <summary>
        ///     Gets the URI templates that are availabe for the service.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the methods available.
        /// </returns>
        public List<string> GetUriTemplates()
        {
            return this.GetUriTemplates(typeof (ChauffeurService));
        }

        /// <summary>
        ///     Gets the configurations.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="ChauffeurConfiguration" /> representing the configuratons.
        /// </returns>
        public Dictionary<string, string> GetConfigurations()
        {
            return this.Configuration.ToDictionary();
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
            var packages = this.GetPackages();
            var package = packages.FirstOrDefault(o => o.Job.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            if (package == null)
            {
                package = new Package
                {
                    Url = new Uri(this.Configuration.PackagesDataFile),
                    Job = jobName,
                };

                packages.Add(package);
            }

            package.BuildNumber = build.Number;
            package.Paths = paths;
            package.Date = DateTime.Now.ToString("f");

            var service = new ChangeSetService(base.BaseUri, base.Client, base.Configuration);
            package.ChangeSet = await service.GetChangesAsync(build);

            this.Serialize(packages);

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
        private async Task<Build> GetBuildAsync(string jobName, string buildNumber)
        {
            var service = new JobService(base.BaseUri, base.Client, base.Configuration);
            return await service.GetBuildAsync(jobName, buildNumber);
        }

        /// <summary>
        ///     Gets the uninstall files from the package cache.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the file from the package cache.</returns>
        private string GetPackageCache()
        {
            if (string.IsNullOrEmpty(this.Configuration.PackageCacheName))
                return null;

            string queryString = string.Format("SELECT * FROM Win32_Product WHERE Name LIKE '{0}%'", this.Configuration.PackageCacheName);
            Log.Info(this, "Loading the installed programs '{0}'", queryString);

            ManagementObjectSearcher mos = new ManagementObjectSearcher(queryString);
            foreach (var mo in mos.Get())
            {
                foreach (var prop in mo.Properties)
                {
                    if (prop.Name == "PackageCache")
                    {
                        string packageCache = (string) prop.Value;
                        if (packageCache != null && File.Exists(packageCache))
                            return packageCache;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the uninstall files from the package cache.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the file from the package cache.</returns>
        private Task<string> GetPackageCacheAsync()
        {
            return Task.Run(() => this.GetPackageCache());
        }

        /// <summary>
        ///     Installs the build MSI packages using the "msiexec.exe" utility on the windows machine.
        /// </summary>
        /// <param name="paths">The packages.</param>
        private void Install(params string[] paths)
        {
            if (paths == null || paths.All(o => o == null)) return;

            Log.Info(this, "Installing {0} package(s).", paths.Length);            

            foreach (var pkg in paths)
            {
                this.WaitForExit(string.Format("/c start /MIN /wait msiexec.exe /i \"{0}\" /quiet {1}", pkg, this.Configuration.InstallPropertyReferences));
            }

            this.WaitForExit(this.Configuration.AfterInstall, this.Configuration.AfterInstallParameters);
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
                if (task.IsFaulted)
                {
                    Log.Error(this, task.Exception);
                }

                Log.Info(this, "Notification: {0}", task.Result);
            });
        }

        /// <summary>
        ///     Saves the packages to the packages data file.
        /// </summary>
        /// <param name="packages">The packages.</param>
        private void Serialize(IEnumerable<Package> packages)
        {
            var json = JsonConvert.SerializeObject(packages, Formatting.Indented);
            File.WriteAllText(base.Configuration.PackagesDataFile, json);
        }

        /// <summary>
        ///     Uninstalls the specified paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        private void Uninstall(params string[] paths)
        {
            if (paths == null || paths.All(o => o == null)) return;

            Log.Info(this, "Uninstalling {0} package(s).", paths.Length);

            this.WaitForExit(this.Configuration.BeforeUninstall, this.Configuration.BeforeUninstallParameters);

            foreach (var pkg in paths)
            {
                this.WaitForExit(string.Format("/c start /MIN /wait msiexec.exe /x \"{0}\" /quiet {1}", pkg, this.Configuration.UninstallPropertyReferences));
            }
        }

        /// <summary>
        ///     Shells the command prompt and waits for it to exit.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        private void WaitForExit(string arguments)
        {
            this.WaitForExit("cmd.exe", arguments);
        }

        /// <summary>
        ///     Shells the command prompt and waits for it to exit.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        private void WaitForExit(string fileName, string arguments)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Log.Info(this, string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

            using (Process process = Process.Start(startInfo))
            {
                if (process != null) process.WaitForExit();
            }
        }

        #endregion
    }
}