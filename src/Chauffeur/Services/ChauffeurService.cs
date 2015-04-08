using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services;

namespace Chauffeur.Services
{
    /// <summary>
    ///     A windows service used to monitor the builds and automatically install the last succsseful build.
    /// </summary>
    public partial class ChauffeurService : ServiceBase
    {
        #region Fields

        private Timer _Timer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurService" /> class.
        /// </summary>
        public ChauffeurService()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Installs the last successful build for the specified job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="artifactDirectory">The directory for the artifacts.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when a new build was installed; otherwise <c>false</c>.
        /// </returns>
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

            // Download the artifacts only when the installed and last successful builds are different.
            string buildNumber = ConfigurationManager.AppSettings["jenkins.build"];
            if (buildNumber == null || buildNumber.ToUpperInvariant() != job.LastSuccessfulBuild.Number.ToString(CultureInfo.CurrentCulture))
            {
                // Download the build artifacts for the job.
                var artifacts = this.DownloadArtifacts(job, artifactDirectory);

                // Install the MSI artifacts.
                this.InstallMsiArtifacts(artifacts);

                // Update the app.config file.
                this.SaveLastSuccessfulBuild(job.LastSuccessfulBuild);

                // A new build was installed.
                return true;
            }

            // Nothing changed.
            return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     When implemented in a derived class, executes when a Start command is sent to the service by the Service Control
        ///     Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to
        ///     take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            double interval;
            if (!double.TryParse(ConfigurationManager.AppSettings["chauffeur.interval"], out interval))
                interval = 900000; // 15 minutes.

            _Timer = new Timer(interval);
            _Timer.Elapsed += Timer_OnElapsed;
            _Timer.Start();
        }

        /// <summary>
        ///     When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control
        ///     Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _Timer.Stop();
            _Timer.Dispose();
            _Timer = null;
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
            string url = ConfigurationManager.AppSettings["jenkins.server"];
            baseUri = new Uri(url);

            string user = ConfigurationManager.AppSettings["jenkins.user"];
            string token = ConfigurationManager.AppSettings["jenkins.token"];

            JsonJenkinsClient client = new JsonJenkinsClient(user, token);
            return client;
        }

        /// <summary>
        ///     Gets the job from the jenkins server.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">jobName</exception>
        private Job GetJob(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            Uri baseUri;
            JenkinsClient client = this.GetClient(out baseUri);

            JobService jobService = new JobService(baseUri, client);
            Job job = jobService.GetJob(jobName);

            return job;
        }

        /// <summary>
        ///     Installs the build artifacts using the "msiexec.exe".
        /// </summary>
        /// <param name="artifacts">The artifacts.</param>
        private void InstallMsiArtifacts(IEnumerable<string> artifacts)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {@"/x", ConfigurationManager.AppSettings["chauffeur.uninstall"]},
                {@"/i", ConfigurationManager.AppSettings["chauffeur.install"]}
            };

            foreach (var artifact in artifacts.Where(o => o.EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase)))
            {
                foreach (var s in parameters)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/c start /MIN /wait msiexec.exe {0} {1} {2}", s.Key, artifact, s.Value));
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    using (Process process = Process.Start(startInfo))
                        if (process != null) process.WaitForExit();
                }
            }
        }

        /// <summary>
        ///     Saves the last successful build.
        /// </summary>
        /// <param name="build">The build.</param>
        private void SaveLastSuccessfulBuild(Build build)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string location = Path.GetDirectoryName(assembly.Location);
            string configFile = Path.Combine(location, assembly.ManifestModule.Name + ".config");
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            config.AppSettings.Settings["jenkins.build"].Value = build.Number.ToString(CultureInfo.CurrentCulture);
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        ///     Handles the OnElapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void Timer_OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _Timer.Stop();

                string jobName = ConfigurationManager.AppSettings["jenkins.job"];
                string artifactsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Jenkins");

                this.InstallLastSuccessfulBuild(jobName, artifactsDirectory);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                _Timer.Start();
            }
        }

        #endregion
    }
}