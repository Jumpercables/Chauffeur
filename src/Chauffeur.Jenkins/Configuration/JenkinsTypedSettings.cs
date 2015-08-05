using System.Collections.Specialized;

namespace Chauffeur.Jenkins.Configuration
{

    #region Nested Type: JenkinsStronglyTypedSettings

    public class JenkinsTypedSettings
    {
        #region Constructors

        public JenkinsTypedSettings(NameValueCollection settings)
        {
            this.Server = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Jenkins/Server", "http://localhost:8080/").Value;
            this.User = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Jenkins/User", "").Value;
            this.Token = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Jenkins/Token", "").Value;
        }

        #endregion

        #region Public Properties

        public string Server { get; private set; }
        public string Token { get; private set; }
        public string User { get; private set; }

        #endregion
    }

    #endregion
}