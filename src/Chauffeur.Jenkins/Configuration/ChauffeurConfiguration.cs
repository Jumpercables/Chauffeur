using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace Chauffeur.Jenkins.Configuration
{
    /// <summary>
    ///     Provides access to the configurations provided in the configuration file.
    /// </summary>
    public sealed class ChauffeurConfiguration
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurConfiguration" /> class.
        /// </summary>
        public ChauffeurConfiguration()
        {
            this.LoadConfigurationAndInitialize(ConfigurationManager.AppSettings.AllKeys.Select(k => Tuple.Create(k, ConfigurationManager.AppSettings[k])).ToList());
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the jenkins.
        /// </summary>
        /// <value>
        ///     The jenkins.
        /// </value>
        public JenkinsConfiguration Jenkins { get; private set; }

        /// <summary>
        ///     Gets the notification.
        /// </summary>
        /// <value>
        ///     The notification.
        /// </value>
        public NotificationsConfiguration Notifications { get; private set; }

        /// <summary>
        ///     Gets the package.
        /// </summary>
        /// <value>
        ///     The package.
        /// </value>
        public PackagesConfiguration Packages { get; private set; }

        /// <summary>
        ///     Gets the resources.
        /// </summary>
        /// <value>
        ///     The resources.
        /// </value>
        public ResourcesConfiguration Resources { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns the configurations as-a dictionary.
        /// </summary>
        /// <returns>Returns a <see cref="Dictionary{String, String}" /> representing the configurations.</returns>
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string>[] dictionaries =
            {
                this.Jenkins.ToDictionary(),
                this.Notifications.ToDictionary(),
                this.Resources.ToDictionary(),
                this.Packages.ToDictionary()
            };

            return dictionaries.SelectMany(dictionary => dictionary).ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Loads the configuration and initialize the settings.
        /// </summary>
        /// <param name="values">The values.</param>
        private void LoadConfigurationAndInitialize(List<Tuple<string, string>> values)
        {
            this.Resources = new ResourcesConfiguration(values);
            this.Jenkins = new JenkinsConfiguration(values);
            this.Notifications = new NotificationsConfiguration(values, this.Resources);
            this.Packages = new PackagesConfiguration(values, this.Resources);
        }

        #endregion

        #region Nested Type: JenkinsConfiguration

        public sealed class JenkinsConfiguration : ApplicationConfiguration
        {
            #region Fields

            private readonly Setting<string> _Server;
            private readonly Setting<string> _Token;
            private readonly Setting<string> _User;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="JenkinsConfiguration" /> class.
            /// </summary>
            /// <param name="values">The values.</param>
            public JenkinsConfiguration(IEnumerable<Tuple<string, string>> values)
                : base(values, "Chauffeur/Jenkins/")
            {
                _Server = new StringSetting(this.Settings, "Chauffeur/Jenkins/Server", "http://localhost:8080/");
                _User = new StringSetting(this.Settings, "Chauffeur/Jenkins/User", "");
                _Token = new StringSetting(this.Settings, "Chauffeur/Jenkins/Token", "");
            }

            #endregion

            #region Public Properties

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
        }

        #endregion

        #region Nested Type: NotificationsConfiguration

        public sealed class NotificationsConfiguration : ApplicationConfiguration
        {
            #region Fields

            private readonly Setting<string> _BodyTemplateFile;
            private readonly Setting<string> _From;
            private readonly Setting<string> _Host;
            private readonly Setting<string> _SubjectTemplateFile;
            private readonly Setting<string> _To;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="NotificationsConfiguration" /> class.
            /// </summary>
            /// <param name="values">The values.</param>
            /// <param name="resources">The resources.</param>
            public NotificationsConfiguration(IEnumerable<Tuple<string, string>> values, ResourcesConfiguration resources)
                : base(values, "Chauffeur/Notifications/")
            {
                _Host = new StringSetting(this.Settings, "Chauffeur/Notifications/Host", "");
                _To = new StringSetting(this.Settings, "Chauffeur/Notifications/To", "");
                _From = new StringSetting(this.Settings, "Chauffeur/Notifications/From", "");
                _SubjectTemplateFile = new StringSetting(this.Settings, "Chauffeur/Notifications/Subject", Path.Combine(resources.TemplateDirectory, "_Notification-Subject.xslt"));
                _BodyTemplateFile = new StringSetting(this.Settings, "Chauffeur/Notifications/Body", Path.Combine(resources.TemplateDirectory, "_Notification-Body.xslt"));
            }

            #endregion

            #region Public Properties

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
            ///     Gets to.
            /// </summary>
            /// <value>
            ///     To.
            /// </value>
            public string To
            {
                get { return _To.Value; }
            }

            #endregion
        }

        #endregion

        #region Nested Type: PackagesConfiguration

        public sealed class PackagesConfiguration : ApplicationConfiguration
        {
            #region Fields

            private readonly Setting<string> _ArtifactsDirectory;
            private readonly Setting<string> _InstallPropertyReferences;
            private readonly Setting<string> _PackageCacheName;
            private readonly Setting<string> _Password;
            private readonly Setting<string> _PowershellPostInstall;
            private readonly Setting<string> _PowershellPreUninstall;
            private readonly Setting<string> _UninstallPropertyReferences;
            private readonly Setting<string> _UserName;
            private readonly Setting<string> _Domain;
 
            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PackagesConfiguration" /> class.
            /// </summary>
            /// <param name="values">The values.</param>
            /// <param name="resources">The resources.</param>
            public PackagesConfiguration(IEnumerable<Tuple<string, string>> values, ResourcesConfiguration resources)
                : base(values, "Chauffeur/Packages/")
            {
                _ArtifactsDirectory = new Setting<string>(this.Settings, "Chauffeur/Packages/Artifacts", value => this.GetDirectory(value, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins")));
                _InstallPropertyReferences = new StringSetting(this.Settings, "Chauffeur/Packages/InstallPropertyReferences", "");
                _UninstallPropertyReferences = new StringSetting(this.Settings, "Chauffeur/Packages/UninstallPropertyReferences", "");
                _PackageCacheName = new StringSetting(this.Settings, "Chauffeur/Packages/PackageCacheName", "");

                _PowershellPreUninstall = new Setting<string>(this.Settings, "Chauffeur/Packages/PreUninstall", value => !File.Exists(value) ? Path.Combine(resources.DataDirectory, "Uninstall.ps1") : value);
                _PowershellPostInstall = new Setting<string>(this.Settings, "Chauffeur/Packages/PostInstall", value => !File.Exists(value) ? Path.Combine(resources.DataDirectory, "Install.ps1") : value);

               _UserName = new StringSetting(this.Settings, "Chauffeur/Packages/User", "");
               _Password = new StringSetting(this.Settings, "Chauffeur/Packages/Password", "");
               _Domain = new StringSetting(this.Settings, "Chauffeur/Packages/Domain", "");               
            }

            #endregion

            #region Public Properties

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
            ///     Gets the after install.
            /// </summary>
            /// <value>
            ///     The after install.
            /// </value>
            public string PowershellPostInstall
            {
                get { return _PowershellPostInstall.Value; }
            }

            /// <summary>
            ///     Gets the before uninstall.
            /// </summary>
            /// <value>
            ///     The before uninstall.
            /// </value>
            public string PowershellPreUninstall
            {
                get { return _PowershellPreUninstall.Value; }
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
            /// Gets a value indicating whether this instance is map drive required.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is map drive required; otherwise, <c>false</c>.
            /// </value>
            public bool IsMapDriveRequired
            {
                get
                {
                    return !string.IsNullOrEmpty(_UserName.Value) && !string.IsNullOrEmpty(_Password.Value);
                }
            }

            /// <summary>
            /// Gets the credentials.
            /// </summary>
            /// <value>
            /// The credentials.
            /// </value>
            public NetworkCredential Credentials
            {
                get
                {                    
                    return new NetworkCredential(_UserName.Value, _Password.Value, _Domain.Value);
                }
            }

            #endregion
        }

        #endregion

        #region Nested Type: ResourcesConfiguration

        public class ResourcesConfiguration : ApplicationConfiguration
        {
            #region Fields

            private readonly Setting<string> _DataDirectory;
            private readonly Setting<string> _PackagesDataFile;
            private readonly Setting<string> _TemplateDirectory;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ResourcesConfiguration" /> class.
            /// </summary>
            /// <param name="values">The values.</param>
            public ResourcesConfiguration(IEnumerable<Tuple<string, string>> values)
                : base(values, "Chauffeur/Resources/")
            {
                _TemplateDirectory = new Setting<string>(this.Settings, "Chauffeur/Resources/Templates", value => this.GetDirectory(value, "~\\Templates"));
                _DataDirectory = new Setting<string>(this.Settings, "Chauffeur/Resources/Data", value => this.GetDirectory(value, "~\\Data"));
                _PackagesDataFile = new StringSetting(this.Settings, "Chauffeur/Resources/Packages", Path.Combine(this.DataDirectory, "Packages.json"));
            }

            #endregion

            #region Public Properties

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
            ///     Gets the package directory.
            /// </summary>
            /// <value>
            ///     The package directory.
            /// </value>
            public string PackagesDataFile
            {
                get { return _PackagesDataFile.Value; }
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

            #endregion
        }

        #endregion
    }

    public abstract class ApplicationConfiguration
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationConfiguration" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="configurationPrefix">The configuration prefix.</param>
        protected ApplicationConfiguration(IEnumerable<Tuple<string, string>> values, string configurationPrefix)
        {
            this.Settings = new NameValueCollection();

            foreach (Tuple<string, string> tuple in values)
            {
                if (tuple.Item1.StartsWith(configurationPrefix, StringComparison.OrdinalIgnoreCase))
                    this.Settings[tuple.Item1] = tuple.Item2;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>
        ///     The settings.
        /// </value>
        protected NameValueCollection Settings { get; set; }

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

        #region Protected Methods

        /// <summary>
        ///     Gets the complete path to the directory.
        /// </summary>
        /// <param name="configurationValue">The configuration value.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the path to the directory.
        /// </returns>
        protected string GetDirectory(string configurationValue, string fallbackValue)
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