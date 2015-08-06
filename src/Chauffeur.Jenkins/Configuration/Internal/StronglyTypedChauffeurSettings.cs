using System;
using System.Collections.Specialized;
using System.IO;

namespace Chauffeur.Jenkins.Configuration
{
    internal class StronglyTypedChauffeurSettings
    {
        #region Constructors

        public StronglyTypedChauffeurSettings()
        {
        }

        #endregion

        #region Public Properties

        public StringSetting Body { get; private set; }
        public StringSetting DataDirectory { get; private set; }
        public StringSetting From { get; private set; }
        public StringSetting Host { get; private set; }
        public StringSetting InstallPropertyReferences { get; private set; }
        public BooleanSetting IsHtml { get; private set; }
        public StringSetting Server { get; private set; }
        public StringSetting Subject { get; private set; }
        public StringSetting To { get; private set; }
        public StringSetting Token { get; private set; }
        public StringSetting UninstallPropertyReferences { get; private set; }
        public StringSetting User { get; private set; }

        #endregion

        #region Public Methods

        public void Initialize(NameValueCollection settings)
        {
            this.Server = new StringSetting(settings, "Chauffeur/Jenkins/Server", "http://localhost:8080/");
            this.User = new StringSetting(settings, "Chauffeur/Jenkins/User", "");
            this.Token = new StringSetting(settings, "Chauffeur/Jenkins/Token", "");

            this.Host = new StringSetting(settings, "Chauffeur/Notifications/Host", "");
            this.To = new StringSetting(settings, "Chauffeur/Notifications/To", "");
            this.From = new StringSetting(settings, "Chauffeur/Notifications/From", "");
            this.IsHtml = new BooleanSetting(settings, "Chauffeur/Notifications/IsHtml", false);
            this.Subject = new StringSetting(settings, "Chauffeur/Notifications/Subject", "");
            this.Body = new StringSetting(settings, "Chauffeur/Notifications/Body", "");

            this.DataDirectory = new StringSetting(settings, "Chauffeur/Packages/DataDirectory", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins"));
            this.InstallPropertyReferences = new StringSetting(settings, "Chauffeur/Packages/InstallPropertyReferences", "/quiet");
            this.UninstallPropertyReferences = new StringSetting(settings, "Chauffeur/Packages/UninstallPropertyReferences", "quiet");
        }

        #endregion
    }


    internal class BooleanSetting : Setting<bool>
    {
        #region Constructors

        public BooleanSetting(NameValueCollection settings, string name, bool value)
            : base(settings, name, value)
        {
        }

        #endregion
    }

    internal class StringSetting : Setting<string>
    {
        #region Constructors

        public StringSetting(NameValueCollection settings, string name, string value)
            : base(settings, name, value)
        {
        }

        #endregion
    }

    internal abstract class Setting<TValue>
    {
        #region Constructors

        protected Setting(NameValueCollection settings, string name, TValue value)
        {
            this.Value = string.IsNullOrEmpty(settings[name]) ? value : (TValue) Convert.ChangeType(settings[name], typeof (TValue));
        }

        #endregion

        #region Public Properties

        public TValue Value { get; private set; }

        #endregion
    }
}