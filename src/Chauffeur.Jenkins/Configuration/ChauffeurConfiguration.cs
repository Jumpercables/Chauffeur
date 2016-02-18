using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Chauffeur.Jenkins.Configuration
{
    /// <summary>
    ///     Provides access to the configurations provided in the configuration file.
    /// </summary>
    public class ChauffeurConfiguration
    {
        #region Fields

        private Setting<string> _AfterInstall;
        private Setting<string> _AfterInstallParameters;
        private Setting<string> _ArtifactsDirectory;
        private Setting<string> _BeforeUninstall;
        private Setting<string> _BeforeUninstallParameters;
        private Setting<string> _BodyTemplateFile;
        private Setting<string> _DataDirectory;
        private Setting<string> _From;
        private Setting<string> _Host;
        private Setting<string> _InstallPropertyReferences;
        private Setting<string> _PackageCacheName;
        private Setting<string> _PackagesDataFile;
        private Setting<string> _Server;
        private Setting<string> _SubjectTemplateFile;
        private Setting<string> _TemplateDirectory;
        private Setting<string> _To;
        private Setting<string> _Token;
        private Setting<string> _UninstallPropertyReferences;
        private Setting<string> _User;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurConfiguration" /> class.
        /// </summary>
        public ChauffeurConfiguration()
        {
            this.LoadConfigurationAndInitialize(ConfigurationManager.AppSettings.AllKeys.Select(k => Tuple.Create(k, ConfigurationManager.AppSettings[k])));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the after install.
        /// </summary>
        /// <value>
        ///     The after install.
        /// </value>
        public string AfterInstall
        {
            get { return _AfterInstall.Value; }
        }

        /// <summary>
        ///     Gets the after install parameters.
        /// </summary>
        /// <value>
        ///     The after install parameters.
        /// </value>
        public string AfterInstallParameters
        {
            get { return _AfterInstallParameters.Value; }
        }

        /// <summary>
        ///     Gets the artifacts directory.
        /// </summary>
        /// <value>
        ///     The artifacts directory.
        /// </value>
        public string ArtifactsDirectory
        {
            get { return _ArtifactsDirectory.Value; }
        }

        /// <summary>
        ///     Gets the before uninstall.
        /// </summary>
        /// <value>
        ///     The before uninstall.
        /// </value>
        public string BeforeUninstall
        {
            get { return _BeforeUninstall.Value; }
        }

        /// <summary>
        ///     Gets the before uninstall parameters.
        /// </summary>
        /// <value>
        ///     The before uninstall parameters.
        /// </value>
        public string BeforeUninstallParameters
        {
            get { return _BeforeUninstallParameters.Value; }
        }

        /// <summary>
        ///     Gets the body.
        /// </summary>
        /// <value>
        ///     The body.
        /// </value>
        public string BodyTemplateFile
        {
            get { return _BodyTemplateFile.Value; }
        }

        /// <summary>
        ///     Gets the data directory.
        /// </summary>
        /// <value>
        ///     The data directory.
        /// </value>
        public string DataDirectory
        {
            get { return _DataDirectory.Value; }
        }

        /// <summary>
        ///     Gets from.
        /// </summary>
        /// <value>
        ///     From.
        /// </value>
        public string From
        {
            get { return _From.Value; }
        }

        /// <summary>
        ///     Gets the host.
        /// </summary>
        /// <value>
        ///     The host.
        /// </value>
        public string Host
        {
            get { return _Host.Value; }
        }

        /// <summary>
        ///     Gets the install property references.
        /// </summary>
        /// <value>
        ///     The install property references.
        /// </value>
        public string InstallPropertyReferences
        {
            get { return _InstallPropertyReferences.Value; }
        }

        /// <summary>
        ///     Gets the name of the package cache.
        /// </summary>
        /// <value>
        ///     The name of the package cache.
        /// </value>
        public string PackageCacheName
        {
            get { return _PackageCacheName.Value; }
        }

        /// <summary>
        ///     Gets the packages json file.
        /// </summary>
        /// <value>
        ///     The packages json file.
        /// </value>
        public string PackagesDataFile
        {
            get { return _PackagesDataFile.Value; }
        }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>
        ///     The server.
        /// </value>
        public string Server
        {
            get { return _Server.Value; }
        }

        /// <summary>
        ///     Gets the subject.
        /// </summary>
        /// <value>
        ///     The subject.
        /// </value>
        public string SubjectTemplateFile
        {
            get { return _SubjectTemplateFile.Value; }
        }

        /// <summary>
        ///     Gets the template directory.
        /// </summary>
        /// <value>
        ///     The template directory.
        /// </value>
        public string TemplateDirectory
        {
            get { return _TemplateDirectory.Value; }
        }

        /// <summary>
        ///     Gets to.
        /// </summary>
        /// <value>
        ///     To.
        /// </value>
        public string To
        {
            get { return _To.Value; }
        }

        /// <summary>
        ///     Gets the token.
        /// </summary>
        /// <value>
        ///     The token.
        /// </value>
        public string Token
        {
            get { return _Token.Value; }
        }

        /// <summary>
        ///     Gets the uninstall property references.
        /// </summary>
        /// <value>
        ///     The uninstall property references.
        /// </value>
        public string UninstallPropertyReferences
        {
            get { return _UninstallPropertyReferences.Value; }
        }

        /// <summary>
        ///     Gets the user.
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public string User
        {
            get { return _User.Value; }
        }

        #endregion

        #region Private Properties

        private NameValueCollection Settings { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns the configurations as-a dictionary.
        /// </summary>
        /// <returns>Returns a <see cref="Dictionary{String, String}" /> representing the configurations.</returns>
        public Dictionary<string, string> ToDictionary()
        {
            return this.Settings.AllKeys.ToDictionary(key => key, key => this.Settings[key]);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the complete path to the directory.
        /// </summary>
        /// <param name="configurationValue">The configuration value.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the path to the directory.
        /// </returns>
        private string GetDirectory(string configurationValue, string fallbackValue)
        {
            configurationValue = configurationValue ?? string.Empty;
            fallbackValue = fallbackValue ?? string.Empty;

            if (string.IsNullOrEmpty(configurationValue))
                configurationValue = fallbackValue;

            if (configurationValue.StartsWith("~"))
                configurationValue = configurationValue.Replace("~", "..");

            string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string path = Path.GetFullPath(Path.Combine(dir, configurationValue));
            return path;
        }

        /// <summary>
        ///     Initializes the strongly typed settings.
        /// </summary>
        private void Initialize()
        {
            _TemplateDirectory = new Setting<string>(this.Settings, "Chauffeur/Resources/Templates", value => this.GetDirectory(value, "~\\Templates"));
            _DataDirectory = new Setting<string>(this.Settings, "Chauffeur/Resources/Data", value => this.GetDirectory(value, "~\\Data"));
            _PackagesDataFile = new StringSetting(this.Settings, "Chauffeur/Resources/Packages", Path.Combine(this.DataDirectory, "Packages.json"));

            _Server = new StringSetting(this.Settings, "Chauffeur/Jenkins/Server", "http://localhost:8080/");
            _User = new StringSetting(this.Settings, "Chauffeur/Jenkins/User", "");
            _Token = new StringSetting(this.Settings, "Chauffeur/Jenkins/Token", "");

            _Host = new StringSetting(this.Settings, "Chauffeur/Notifications/Host", "");
            _To = new StringSetting(this.Settings, "Chauffeur/Notifications/To", "");
            _From = new StringSetting(this.Settings, "Chauffeur/Notifications/From", "");
            _SubjectTemplateFile = new StringSetting(this.Settings, "Chauffeur/Notifications/Subject", Path.Combine(this.TemplateDirectory, "_Notification-Subject.xslt"));
            _BodyTemplateFile = new StringSetting(this.Settings, "Chauffeur/Notifications/Body", Path.Combine(this.TemplateDirectory, "_Notification-Body.xslt"));

            _ArtifactsDirectory = new Setting<string>(this.Settings, "Chauffeur/Packages/Artifacts", value => this.GetDirectory(value, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins")));
            _InstallPropertyReferences = new StringSetting(this.Settings, "Chauffeur/Packages/InstallPropertyReferences", "");
            _UninstallPropertyReferences = new StringSetting(this.Settings, "Chauffeur/Packages/UninstallPropertyReferences", "");
            _PackageCacheName = new StringSetting(this.Settings, "Chauffeur/Packages/PackageCacheName", "");
            _BeforeUninstall = new StringSetting(this.Settings, "Chauffeur/Packages/BeforeInstall", "");
            _AfterInstall = new StringSetting(this.Settings, "Chauffeur/Packages/AfterInstall", "");
            _BeforeUninstallParameters = new StringSetting(this.Settings, "Chauffeur/Packages/BeforeUninstallParameters", "");
            _AfterInstallParameters = new StringSetting(this.Settings, "Chauffeur/Packages/AfterInstallParameters", "");
        }

        /// <summary>
        ///     Loads the configuration and initialize the settings.
        /// </summary>
        /// <param name="values">The values.</param>
        private void LoadConfigurationAndInitialize(IEnumerable<Tuple<string, string>> values)
        {
            this.Settings = new NameValueCollection();

            foreach (Tuple<string, string> tuple in values)
            {
                if (tuple.Item1.StartsWith("Chauffeur/", StringComparison.OrdinalIgnoreCase))
                    this.Settings[tuple.Item1] = tuple.Item2;
            }

            this.Initialize();
        }

        #endregion

        #region Nested Type: BooleanSetting

        internal class BooleanSetting : Setting<bool>
        {
            #region Constructors

            public BooleanSetting(NameValueCollection settings, string name, bool value)
                : base(settings, name, s => string.IsNullOrEmpty(s) ? value : bool.Parse(s))
            {
            }

            #endregion
        }

        #endregion

        #region Nested Type: IntegerSetting

        internal class IntegerSetting : Setting<int>
        {
            #region Constructors

            public IntegerSetting(NameValueCollection settings, string name, int value)
                : base(settings, name, s => string.IsNullOrEmpty(s) ? value : int.Parse(s))
            {
            }

            #endregion
        }

        #endregion

        #region Nested Type: Setting

        internal class Setting<TValue>
        {
            #region Constructors

            public Setting(NameValueCollection settings, string name, Func<string, TValue> func, Action<NameValueCollection, TValue> action)
            {
                this.Name = name;
                this.Value = func(settings[name]);

                action(settings, this.Value);
            }

            public Setting(NameValueCollection settings, string name, Func<string, TValue> func)
                : this(settings, name, func, (collection, value) => collection[name] = value.ToString())
            {
            }

            #endregion

            #region Public Properties

            public string Name { get; private set; }
            public TValue Value { get; private set; }

            #endregion
        }

        #endregion

        #region Nested Type: StringSetting

        internal class StringSetting : Setting<string>
        {
            #region Constructors

            public StringSetting(NameValueCollection settings, string name, string value)
                : base(settings, name, s => string.IsNullOrEmpty(s) ? value : s)
            {
            }

            #endregion
        }

        #endregion
    }
}