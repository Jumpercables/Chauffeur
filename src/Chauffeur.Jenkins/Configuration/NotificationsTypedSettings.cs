using System.Collections.Specialized;

namespace Chauffeur.Jenkins.Configuration
{

    #region Nested Type: NotificationsStronglyTypedSettings

    public class NotificationsTypedSettings
    {
        #region Constructors

        public NotificationsTypedSettings(NameValueCollection settings)
        {
            this.Host = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Notifications/Host", "").Value;
            this.To = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Notifications/To", "").Value;
            this.From = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Notifications/From", "").Value;
            this.IsHtml = new ChauffeurConfiguration.BooleanSetting(settings, "Chauffeur/Notifications/IsHtml", false).Value;
            this.Subject = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Notifications/Subject", "").Value;
            this.Body = new ChauffeurConfiguration.StringSetting(settings, "Chauffeur/Notifications/Body", "").Value;
        }

        #endregion

        #region Public Properties

        public string Body { get; private set; }
        public string From { get; private set; }
        public string Host { get; private set; }
        public bool IsHtml { get; private set; }
        public string Subject { get; private set; }
        public string To { get; private set; }

        #endregion
    }

    #endregion
}