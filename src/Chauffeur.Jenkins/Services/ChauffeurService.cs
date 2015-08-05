using System.Collections.Generic;
using System.Diagnostics;
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
        [WebGet(UriTemplate = "Install/{jobName}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
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
            // Query jenkins for the build information.
            var build = await this.GetBuild(jobName);

            // Download the packages.
            var packages = await this.DownloadPackages(build);

            // Install the packages.
            this.InstallPackages(packages);

            // Send the notifications.
            this.Notify(build);

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
            var service = new ArtifactService(base.BaseUri, base.Client);
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
            var service = new JobService(base.BaseUri, base.Client);
            return await service.GetLastSuccessfulBuildAsync(jobName);
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
        /// <param name="packages">The packages.</param>
        private async void InstallPackages(string[] packages)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {@"/x", this.Configuration.Packages.UninstallCommandLineOptions},
                {@"/i", this.Configuration.Packages.InstallCommandLineOptions}
            };

            this.Log("Installing {0} package(s).", packages.Length);

            foreach (var pkg in packages)
            {
                foreach (var p in parameters)
                {
                    await this.InstallPackageAsync(pkg, p);
                }
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

        #endregion
    }
}