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

            // Assume the build installed is different.
            this.Log("Last successful build: {0}", job.LastSuccessfulBuild.Number);

            // Download the build artifacts for the job.
            string artifactsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");
            var artifacts = this.DownloadArtifacts(job, artifactsDirectory);

            // Install the MSI artifacts.
            this.InstallMsiArtifacts(artifacts.Where(o => o.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase)).ToList());

            // Return the build installed.
            return job.LastSuccessfulBuild;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Downloads the artifacts to the specified directory.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{String}" /> representing the paths to the artifacts that have been downloaded.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     job
        ///     or
        ///     directory
        /// </exception>
        private IEnumerable<string> DownloadArtifacts(Job job, string directory)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            if (directory == null)
                throw new ArgumentNullException("directory");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            ArtifactService artifactService = new ArtifactService(base.BaseUri, base.Client);
            return artifactService.DownloadArtifacts(job.LastSuccessfulBuild, directory);
        }

        /// <summary>
        ///     Installs the build artifacts using the "msiexec.exe".
        /// </summary>
        /// <param name="artifacts">The artifacts.</param>
        private void InstallMsiArtifacts(List<string> artifacts)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {@"/x", ConfigurationManager.AppSettings["uninstall"]},
                {@"/i", ConfigurationManager.AppSettings["install"]}
            };

            this.Log("Installation: {0}", artifacts.Count);

            foreach (var artifact in artifacts)
            {
                foreach (var s in parameters)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} {1} {2}", s.Key, artifact, s.Value));
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