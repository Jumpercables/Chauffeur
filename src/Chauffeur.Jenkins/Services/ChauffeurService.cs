using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Model;

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
        ///     Download and installs the last successful build on the machine that host the service.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Install/{jobName}", ResponseFormat = WebMessageFormat.Json, RequestFormat =  WebMessageFormat.Json)]
        Task<Build> InstallBuildAsync(string jobName);

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
            // Query the server for the last successful build for the job.         
            var service = new JobService(base.BaseUri, base.Client);
            var build = await service.GetBuildAsync(jobName);

            // Download the packages.
            var packages = await this.DownloadPackages(build);

            // Install the packages.
            this.InstallPackages(build, packages);

            // Return the build.
            return build;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        ///     Downloads the packages of the build.
        /// </summary>
        /// <param name="build">The build.</param>
        private async Task<string[]> DownloadPackages(Build build)
        {
            // Install the last successful build but move to the next method and don't wait for completion. 
            var directory = this.GetPackagesPath();            

            // Download the build artifacts for the job.
            var service = new ArtifactService(base.BaseUri, base.Client);
            var packages = await service.DownloadArtifactsAsync(build, directory);

            return packages;
        }

        /// <summary>
        ///     Gets the packages path.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the path to the artifacts directory.</returns>
        private string GetPackagesPath()
        {
            // Use the directory specified in the app.config, otherwise use the fallback directory.
            var directory = ConfigurationManager.AppSettings["chauffuer.packages"];
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");
                this.Log("The 'chauffuer.packages' directory does not exist (using default directory: {0})", directory);
            }

            return directory;
        }

        /// <summary>
        ///     Executes the installation or de-installation of the package based on the parameters.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     Returns the <see cref="Task" /> representing the operation.
        /// </returns>
        private Task InstallPackageAsync(string package, KeyValuePair<string, string> parameters)
        {
            return Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} \"{1}\" {2}", parameters.Key, package, parameters.Value));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                this.Log(string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

                using (Process process = Process.Start(startInfo))
                    if (process != null) process.WaitForExit();
            });
        }

        /// <summary>
        ///     Installs the build MSI packages using the "msiexec.exe" utility on the windows machine.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="packages">The packages.</param>
        private async void InstallPackages(Build build, string[] packages)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {@"/x", ConfigurationManager.AppSettings["chauffeur.uninstall"] ?? "/quiet"},
                {@"/i", ConfigurationManager.AppSettings["chauffeur.install"] ?? "/quiet"}
            };

            this.Log("Installing {0} package(s).", packages.Length);

            foreach (var pkg in packages)
            {
                foreach (var p in parameters)
                {
                    await this.InstallPackageAsync(pkg, p);
                }
            }

            NotificationService service = new NotificationService();
            await service.SendAsync(build).ContinueWith((task) => this.Log("Notification: {0}", task.Result));
        }

        #endregion
    }   
}