using System;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Configuration;
using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services.Templates;

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
                    var addresses = this.Configuration.To.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var address in addresses)
                        message.To.Add(new MailAddress(address.Trim()));

                    message.From = new MailAddress(this.Configuration.From);
                    message.Subject = StyleSheetTemplate.ApplyTemplate(this.Configuration.SubjectTemplateFile, package);
                    message.Body = StyleSheetTemplate.ApplyTemplate(this.Configuration.BodyTemplateFile, package);
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