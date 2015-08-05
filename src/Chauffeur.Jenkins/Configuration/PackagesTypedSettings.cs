using System;
using System.Collections.Specialized;
using System.IO;

namespace Chauffeur.Jenkins.Configuration
{

    #region Nested Type: PackagesStronglyTypedSettings

    public class PackagesTypedSettings
    {
        #region Constructors

        public PackagesTypedSettings(NameValueCollection settings)
        {
            this.DownloadDirectory = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Packages/DownloadDirectory", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins")).Value;
            this.InstallCommandLineOptions = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Packages/InstallCommandLineOptions", "/quiet").Value;
            this.UninstallCommandLineOptions = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Packages/UninstallCommandLineOptions", "quiet").Value;
        }

        #endregion

        #region Public Properties

        public string DownloadDirectory { get; private set; }
        public string InstallCommandLineOptions { get; private set; }
        public string UninstallCommandLineOptions { get; private set; }

        #endregion
    }

    #endregion
}