using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
        [OperationContract]
        bool InstallLastSuccessfulBuild(string jobName);

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
        [OperationContract]
        bool InstallLastSuccessfulBuild(string jobName, string artifactDirectory);

        #endregion
    }

    /// <summary>
    ///     Provides a WCF service that will install builds on the host machines.
    /// </summary>
    public class ChauffeurService : IChauffeurService
    {
        #region IChauffeurService Members

        /// <summary>
        ///     Installs the last successful build for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
        public bool InstallLastSuccessfulBuild(string jobName)
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
        public bool InstallLastSuccessfulBuild(string jobName, string artifactDirectory)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            if (artifactDirectory == null)
                throw new ArgumentNullException("artifactDirectory");

            // Query for the job information from the server.
            Job job = this.GetJob(jobName);

            string buildNumber = ConfigurationManager.AppSettings["build"];
            this.Log("Last successful build: {0}", job.LastSuccessfulBuild.Number);
            this.Log("Last installed build: {0}", buildNumber);

            // Download the artifacts only when the installed and last successful builds are different.           
            if (buildNumber == null || buildNumber.ToUpperInvariant() != job.LastSuccessfulBuild.Number.ToString(CultureInfo.CurrentCulture))
            {
                // Download the build artifacts for the job.
                var artifacts = this.DownloadArtifacts(job, artifactDirectory);

                // Install the MSI artifacts.
                this.InstallMsiArtifacts(artifacts);

                // Update the app.config file.
                this.SaveLastInstalledBuild(job.LastSuccessfulBuild);

                // A new build was installed.
                return true;
            }

            // Nothing changed.
            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the job from the jenkins server.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">jobName</exception>
        public Job GetJob(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            this.Log("Job: {0}", jobName);

            Uri baseUri;
            JenkinsClient client = this.GetClient(out baseUri);

            JobService jobService = new JobService(baseUri, client);
            Job job = jobService.GetJob(jobName);
            return job;
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

            this.Log("Downloading: {0}", directory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Uri baseUri;
            JenkinsClient client = this.GetClient(out baseUri);

            ArtifactService artifactService = new ArtifactService(baseUri, client);
            return artifactService.DownloadArtifacts(job.LastSuccessfulBuild, directory);
        }

        /// <summary>
        ///     Creates the jenkins client.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>Returns the <see cref="JenkinsClient" /> representing the client object.</returns>
        private JenkinsClient GetClient(out Uri baseUri)
        {
            string url = ConfigurationManager.AppSettings["server"];
            baseUri = new Uri(url);

            string user = ConfigurationManager.AppSettings["user"];
            string token = ConfigurationManager.AppSettings["token"];

            JsonJenkinsClient client = new JsonJenkinsClient(user, token);
            return client;
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

        /// <summary>
        ///     Logs the message in the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        private void Log(string format, params object[] args)
        {
            Trace.WriteLine(string.Format("{0} - {1}", DateTime.Now, string.Format(format, args)));
        }


        /// <summary>
        ///     Saves the last installed build.
        /// </summary>
        /// <param name="build">The build.</param>
        private void SaveLastInstalledBuild(Build build)
        {
            this.Log("Last installed build: {0}", build.Number);

            Assembly assembly = Assembly.GetExecutingAssembly();
            string location = Path.GetDirectoryName(assembly.Location);
            if (location == null) return;

            string configFile = Path.Combine(location, assembly.ManifestModule.Name + ".config");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains("build"))
                config.AppSettings.Settings["build"].Value = build.Number.ToString(CultureInfo.CurrentCulture);
            else
                config.AppSettings.Settings.Add("build", build.Number.ToString(CultureInfo.CurrentCulture));

            config.Save(ConfigurationSaveMode.Modified);
        }

        #endregion
    }
}