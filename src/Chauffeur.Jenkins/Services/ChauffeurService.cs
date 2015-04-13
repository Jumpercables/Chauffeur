using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;

using Chauffeur.Jenkins.Client;
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

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build that was installed.
        /// </returns>
        [OperationContract]
        Build InstallLastSuccessfulBuild(string jobName, string artifactDirectory);

        #endregion
    }

    /// <summary>
    ///     Provides a WCF service that will install builds on the host machines.
    /// </summary>
    public class ChauffeurService : JenkinsService, IChauffeurService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        public ChauffeurService(Uri baseUri, JenkinsClient client)
            : base(baseUri, client)
        {
        }

        #endregion

        #region IChauffeurService Members

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
        public Build InstallLastSuccessfulBuild(string jobName)
        {
            string artifactsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");

            return this.InstallLastSuccessfulBuild(jobName, artifactsDirectory);
        }

        /// <summary>
        ///     Installs the last successful build for the specified job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     jobName
        ///     or
        ///     artifactDirectory
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     jobName
        ///     or
        ///     artifactDirectory
        /// </exception>
        public Build InstallLastSuccessfulBuild(string jobName, string artifactDirectory)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            if (artifactDirectory == null)
                throw new ArgumentNullException("artifactDirectory");

            // Query for the job information from the server.            
            JobService jobService = new JobService(base.BaseUri, base.Client);
            Job job = jobService.GetJob(jobName);

            // Assume the build installed is different.
            this.Log("Last successful build: {0}", job.LastSuccessfulBuild.Number);

            // Download the build artifacts for the job.
            var artifacts = this.DownloadArtifacts(job, artifactDirectory);

            // Install the MSI artifacts.
            this.InstallMsiArtifacts(artifacts);

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
        private List<string> DownloadArtifacts(Job job, string directory)
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

            foreach (var artifact in artifacts.Where(o => o.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase)))
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