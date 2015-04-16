using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
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
        ///     Installs the build asynchronous on the machine that host the service.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>
        ///     Returns a <see cref="Task" /> for the asynchrous task.
        /// </returns>
        [OperationContract]
        void InstallBuild(Build build);

        /// <summary>
        ///     Installs the last successful build asynchronous on the machine that host the service.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        [OperationContract]
        Build InstallLastSuccessfulBuild(string jobName);

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
        ///     Installs the build on the machine that hosts the service.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <exception cref="System.ServiceModel.FaultException">The build was not provided.</exception>
        public void InstallBuild(Build build)
        {
            if (build == null)
                throw new FaultException("The build was not provided.");

            // Download the build artifacts for the job.
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var jenkinsService = new ArtifactService(base.BaseUri, base.Client);
            var artifacts = jenkinsService.DownloadArtifacts(build, directory);

            // Install the MSI packages on a thread to prevent blocking.
            this.InstallPackagesAsync(artifacts.Where(o => o.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase)).ToList());            

            // The install completes successfully, set the environment variable.
            Environment.SetEnvironmentVariable("JENKINS_BUILD_INSTALLED", build.Number.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns the <see cref="Build" /> that was installed on the machine.
        /// </returns>
        /// <exception cref="System.ServiceModel.FaultException">The job name was not provided.</exception>
        public Build InstallLastSuccessfulBuild(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new FaultException("The job name was not provided.");

            // Query for the job information from the server.            
            var jenkinsService = new JobService(base.BaseUri, base.Client);
            Job job = jenkinsService.GetJob(jobName);

            // The caller should know the build installed or use the JobService to confirm a new build is needed.
            this.Log("Last successful build: {0}", job.LastSuccessfulBuild.Number);

            // Install the last successful build but move to the next method and don't wait for completion.
            this.InstallBuild(job.LastSuccessfulBuild);

            // Return the build installed.
            return job.LastSuccessfulBuild;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Installs the build MSI packages using the "msiexec.exe" utility on the windows machine.
        /// </summary>
        /// <param name="packages">The packages.</param>
        private void InstallPackages(List<string> packages)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {@"/x", ConfigurationManager.AppSettings["uninstall"]},
                {@"/i", ConfigurationManager.AppSettings["install"]}
            };

            this.Log("Installing {0} package(s).", packages.Count);

            foreach (var pkg in packages)
            {
                foreach (var s in parameters)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} \"{1}\" {2}", s.Key, pkg, s.Value));
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    this.Log(string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

                    using (Process process = Process.Start(startInfo))
                        if (process != null) process.WaitForExit();
                }
            }
        }

        /// <summary>
        ///     Installs the packages asynchronous.
        /// </summary>
        /// <param name="packages">The packages.</param>
        private void InstallPackagesAsync(List<string> packages)
        {
            Task.Run(() => this.InstallPackages(packages));
        }

        #endregion
    }
}