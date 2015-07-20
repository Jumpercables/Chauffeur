using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text.RegularExpressions;

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
        bool Send(Build build);

        #endregion
    }

    /// <summary>
    ///     Provides a WFC contract that will send out a notification for the build.
    /// </summary>
    public class NotificationService : INotificationService
    {
        #region Constants

        private const string BUILD_PROPERTY = "{BUILD}";
        private const string ENVIRONMENT_PROPERTY = "{ENVIRONMENT}";

        #endregion

        #region INotificationService Members

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
        public bool Send(Build build)
        {
            string to = ConfigurationManager.AppSettings["email.to"];
            if (string.IsNullOrEmpty(to)) return false;

            string host = ConfigurationManager.AppSettings["email.host"];
            if (string.IsNullOrEmpty(host))
                throw new FaultException("The 'host' must be in the configuration file.");

            string from = ConfigurationManager.AppSettings["email.from"];
            if (string.IsNullOrEmpty(host))
                throw new FaultException("The 'from' must be in the configuration file.");

            using (MailMessage message = new MailMessage())
            {
                message.To.Add(to);
                message.From = new MailAddress(from);
                message.Subject = this.ApplyTemplates(build, ConfigurationManager.AppSettings["email.subject"]);;                
                message.Body = this.ApplyTemplates(build, ConfigurationManager.AppSettings["email.body"]);;

                using (SmtpClient client = new SmtpClient(host))
                    client.Send(message);
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the templates to the data string.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="data">The data.</param>
        /// <returns>Returns</returns>
        private string ApplyTemplates(Build build, string data)
        {
            Dictionary<string, string>[] templates =
            {
                this.GetBuildTemplate(build), 
                this.GetChauffeurTemplate()
            };

            return templates.Aggregate(data, this.ApplyTemplate);
        }

        /// <summary>
        /// Applies the template to the data string.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        private string ApplyTemplate(string data, Dictionary<string,string> template)
        {
            return template.Aggregate(data, (current, t) => Regex.Replace(current, t.Key, t.Value));
        }

        /// <summary>
        /// Gets the build template.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>Returns a <see cref="Dictionary{String, String}"/> representing the build template.</returns>
        private Dictionary<string, string> GetBuildTemplate(Build build)
        {
            Dictionary<string, string> template = new Dictionary<string, string>();
            
            Type t = build.GetType();
            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(string) || p.PropertyType == typeof(bool) || p.PropertyType == typeof(Uri))
                {
                    string key = @"\{" + string.Format(@"BUILD\({0}\)", p.Name.ToUpperInvariant()) + @"\}";
                    string value = string.Format("{0}", p.GetValue(build));
                    template.Add(key, value);
                }
            }

            return template;
        }

        /// <summary>
        /// Gets the chauffeur template.
        /// </summary>
        /// <returns>Returns a <see cref="Dictionary{String, String}"/> representing the chauffeur template.</returns>
        private Dictionary<string, string> GetChauffeurTemplate()
        {
            Dictionary<string, string> template = new Dictionary<string, string>();
            template.Add(@"\{CHAUFFEUR\(MACHINENAME\)\}", Environment.MachineName);
            
            return template;
        } 

        #endregion
    }
}