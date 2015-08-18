﻿using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

using Chauffeur.Jenkins.Configuration;
using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Templates;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     Provides a WFC contract that will send out a notification for the build.
    /// </summary>
    [ServiceContract]
    public interface INotificationService
    {
        #region Public Methods

        /// <summary>
        ///     Sends a notification that the package was installed.
        /// </summary>
        /// <param name="package">The build.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> when the notification was sent.
        /// </returns>
        [OperationContract]
        Task<bool> SendAsync(Package package);

        #endregion
    }

    /// <summary>
    ///     Provides a WFC contract that will send out a notification for the build.
    /// </summary>
    public class NotificationService : INotificationService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationService" /> class.
        /// </summary>
        public NotificationService()
        {
            this.Configuration = new ChauffeurConfiguration();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        protected ChauffeurConfiguration Configuration { get; set; }

        #endregion

        #region INotificationService Members

        /// <summary>
        ///     Sends a notification of the specified build.
        /// </summary>
        /// <param name="package">The build.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> when the notification was sent.
        /// </returns>
        public Task<bool> SendAsync(Package package)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(this.Configuration.To))
                    return false;

                if (string.IsNullOrEmpty(this.Configuration.Host))
                    throw new WebFaultException<ErrorData>(new ErrorData("The 'host' must be provided.", "The configuration must be provided in the configuration file."), HttpStatusCode.NotFound);

                if (string.IsNullOrEmpty(this.Configuration.From))
                    throw new WebFaultException<ErrorData>(new ErrorData("The 'from' must be provided.", "The configuration must be provided in the configuration file."), HttpStatusCode.NotFound);

                using (MailMessage message = new MailMessage())
                {
                    message.To.Add(this.Configuration.To);
                    message.From = new MailAddress(this.Configuration.From);
                    message.Subject = new StyleSheetTemplate<Package>(this.Configuration.SubjectXslFile).ApplyTemplate(package);
                    message.Body = new StyleSheetTemplate<Package>(this.Configuration.BodyXslFile).ApplyTemplate(package);
                    message.IsBodyHtml = true;

                    using (SmtpClient client = new SmtpClient(this.Configuration.Host))
                        client.Send(message);
                }

                return true;
            });
        }

        #endregion
    }    
}