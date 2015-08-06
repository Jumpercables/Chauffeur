using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Chauffeur.Jenkins.Configuration
{
    public class ChauffeurConfiguration
    {
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

        public string Body { get; private set; }
        public string PackagesJsonFile
        {
            get { return Path.Combine(this.DataDirectory, "Packages.json"); }
        }
        public string DataDirectory { get; private set; }
        public string From { get; private set; }
        public string Host { get; private set; }
        public string InstallPropertyReferences { get; private set; }
        public bool IsHtml { get; private set; }
        public string Server { get; private set; }
        public string Subject { get; private set; }
        public string To { get; private set; }
        public string Token { get; private set; }
        public string UninstallPropertyReferences { get; private set; }
        public string User { get; private set; }

        #endregion

        #region Private Properties

        private NameValueCollection Settings { get; set; }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            StronglyTypedChauffeurSettings typedChauffeurSettings = new StronglyTypedChauffeurSettings();
            typedChauffeurSettings.Initialize(this.Settings);

            this.Server = typedChauffeurSettings.Server.Value;
            this.User = typedChauffeurSettings.User.Value;
            this.Token = typedChauffeurSettings.Token.Value;

            this.Host = typedChauffeurSettings.Host.Value;
            this.To = typedChauffeurSettings.To.Value;
            this.From = typedChauffeurSettings.From.Value;
            this.IsHtml = typedChauffeurSettings.IsHtml.Value;
            this.Subject = typedChauffeurSettings.Subject.Value;
            this.Body = typedChauffeurSettings.Body.Value;

            this.DataDirectory = typedChauffeurSettings.DataDirectory.Value;
            this.InstallPropertyReferences = typedChauffeurSettings.InstallPropertyReferences.Value;
            this.UninstallPropertyReferences = typedChauffeurSettings.UninstallPropertyReferences.Value;
        }

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
    }
}