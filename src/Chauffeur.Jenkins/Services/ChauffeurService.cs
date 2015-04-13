using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;

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
        ///     Installs the build on the machine that hosts the service.
        /// </summary>
        /// <param name="build">The build.</param>
        [OperationContract]
        void InstallBuild(Build build);

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build was installed.
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
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">jobName</exception>
        public Build InstallLastSuccessfulBuild(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            // Query for the job information from the server.            
            JobService jobService = new JobService(base.BaseUri, base.Client);
            Job job = jobService.GetJob(jobName);

            // The caller should know the build installed or use the JobService to confirm a new build is needed.
            this.Log("Last successful build: {0}", job.LastSuccessfulBuild.Number);

            // Install the last successful build.
            this.InstallBuild(job.LastSuccessfulBuild);

            // Return the build installed.
            return job.LastSuccessfulBuild;
        }

        /// <summary>
        ///     Installs the build on the machine that hosts the service.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <exception cref="System.ArgumentNullException">build</exception>
        public void InstallBuild(Build build)
        {
            if (build == null)
                throw new ArgumentNullException("build");

            // Download the build artifacts for the job.
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");
            var artifacts = this.DownloadArtifacts(build, directory);

            // Install only the MSI packages.
            this.InstallPackages(artifacts.Where(o => o.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase)).ToList());
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Downloads the build artifacts to the specified directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{String}" /> representing the paths to the artifacts that have been downloaded.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     job
        ///     or
        ///     directory
        /// </exception>
        private IEnumerable<string> DownloadArtifacts(Build build, string directory)
        {
            if (build == null)
                throw new ArgumentNullException("build");

            if (directory == null)
                throw new ArgumentNullException("directory");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            ArtifactService artifactService = new ArtifactService(base.BaseUri, base.Client);
            return artifactService.DownloadArtifacts(build, directory);
        }

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
                    ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} {1} {2}", s.Key, pkg, s.Value));
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    this.Log(string.Format("\t{0} {1}", startInfo.FileName, startInfo.Arguments));

                    using (Process process = Process.Start(startInfo))
                        if (process != null) process.WaitForExit();
                }
            }
        }

        #endregion
    }
}