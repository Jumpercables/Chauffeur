using System;
using System.Configuration;
using System.Net.Mail;
using System.ServiceModel;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Model;

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
        ///     Sends a notification of the specified build.
        /// </summary>
        /// <param name="build">The build.</param>
        [OperationContract]
        Task SendAsync(Build build);

        #endregion
    }

    /// <summary>
    ///     Provides a WFC contract that will send out a notification for the build.
    /// </summary>
    public class NotificationService : INotificationService
    {
        #region IEmailService Members

        /// <summary>
        ///     Sends a notification of the specified build.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <exception cref="System.ServiceModel.FaultException">
        ///     The 'host' must be in the configuration file.
        ///     or
        ///     The 'from' must be in the configuration file.
        /// </exception>
        /// <remarks>
        ///     When the 'to' configuration is not provided in the configuration file, the notification will not be sent.
        /// </remarks>
        public Task SendAsync(Build build)
        {
            string to = ConfigurationManager.AppSettings["email.to"];
            if (string.IsNullOrEmpty(to)) return null;

            string host = ConfigurationManager.AppSettings["email.host"];
            if (string.IsNullOrEmpty(host))
                throw new FaultException("The 'host' must be in the configuration file.");

            string from = ConfigurationManager.AppSettings["email.from"];
            if (string.IsNullOrEmpty(host))
                throw new FaultException("The 'from' must be in the configuration file.");

            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.Subject = "Build: " + build.Number + " installed on " + Environment.MachineName;
            message.From = new MailAddress(from);
            message.Body = ConfigurationManager.AppSettings["email.body"];

            using (SmtpClient client = new SmtpClient(host))
                return client.SendMailAsync(message); 
            
        }

        #endregion
    }
}