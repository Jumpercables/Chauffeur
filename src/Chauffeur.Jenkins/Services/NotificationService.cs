using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
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
    public class NotificationService : JenkinsService, INotificationService
    {
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

                var templates = this.GetTemplates(package);
                var subject = this.ApplyTemplates(this.Configuration.Subject, templates);
                var body = this.GetBody(templates);

                using (MailMessage message = new MailMessage())
                {
                    message.To.Add(this.Configuration.To);
                    message.From = new MailAddress(this.Configuration.From);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = this.Configuration.IsHtml;

                    using (SmtpClient client = new SmtpClient(this.Configuration.Host))
                        client.Send(message);
                }

                return true;
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Applies the template to the data string.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        private string ApplyTemplate(string data, Dictionary<string, string> template)
        {
            return template.Aggregate(data, (current, t) => Regex.Replace(current, t.Key, t.Value));
        }

        /// <summary>
        ///     Replaces all of the occurances of the tokens in the string with the corresponding values.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="templates">The templates.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the data replaced with the tokens.
        /// </returns>
        private string ApplyTemplates(string data, IEnumerable<Dictionary<string, string>> templates)
        {
            return templates.Aggregate(data, this.ApplyTemplate);
        }

        /// <summary>
        ///     Gets the body of the notification.
        /// </summary>
        /// <param name="templates">The templates.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the body.
        /// </returns>
        private string GetBody(IEnumerable<Dictionary<string, string>> templates)
        {
            if (this.Configuration.IsHtml)
            {
                if (File.Exists(this.Configuration.Body))
                {
                    string value = File.ReadAllText(this.Configuration.Body);
                    return this.ApplyTemplates(value, templates);
                }
            }

            return this.Configuration.Body;
        }

        /// <summary>
        ///     Gets the change set template.
        /// </summary>
        /// <param name="changeSet">The change set.</param>
        /// <returns>Returns a <see cref="Dictionary{String, String}" /> representing the property template.</returns>
        private Dictionary<string, string> GetChangeSetTemplate(ChangeSet changeSet)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            if (changeSet != null)
            {
                foreach (var changeSetItem in changeSet.Items)
                {
                    var t = this.GetPropertyTemplate(changeSetItem, "CHANGESET");
                    foreach (var o in t)
                    {
                        if (!list.ContainsKey(o.Key))
                        {
                            list.Add(o.Key, o.Value);
                        }
                        else
                        {
                            list[o.Key] = string.Concat(list[o.Key], Environment.NewLine, o.Value);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Gets the chauffeur template.
        /// </summary>
        /// <returns>Returns a <see cref="Dictionary{String, String}" /> representing the chauffeur template.</returns>
        private Dictionary<string, string> GetChauffeurTemplate()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add(@"\{CHAUFFEUR\(MACHINENAME\)\}", Environment.MachineName);

            return list;
        }

        /// <summary>
        ///     Gets the property template.
        /// </summary>
        /// <param name="o">The object with public properties.</param>
        /// <param name="token">The token.</param>
        /// <returns>Returns a <see cref="Dictionary{String, String}" /> representing the property template.</returns>
        private Dictionary<string, string> GetPropertyTemplate(object o, string token)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            if (o != null)
            {
                Type t = o.GetType();
                var props = t.GetProperties();
                foreach (var p in props)
                {
                    string key = @"\{" + string.Format(@"{0}\({1}\)", token, p.Name.ToUpperInvariant()) + @"\}";

                    if (p.PropertyType == typeof (int) || p.PropertyType == typeof (string) || p.PropertyType == typeof (bool) || p.PropertyType == typeof (Uri))
                    {
                        string value = string.Format("{0}", p.GetValue(o));
                        list.Add(key, value);
                    }
                    else if (p.PropertyType == typeof (IList<string>))
                    {
                        IList<string> values = p.GetValue(o) as IList<string>;
                        if (values != null)
                        {
                            string value = string.Format("{0}", string.Join(Environment.NewLine, values));
                            list.Add(key, value);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Gets the templates.
        /// </summary>
        /// <param name="package">The build.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, String}" /> representing the templates.
        /// </returns>
        private Dictionary<string, string>[] GetTemplates(Package package)
        {
            Dictionary<string, string>[] templates =
            {
                this.GetPropertyTemplate(package, "PACKAGE"),
                this.GetPropertyTemplate(package.Build, "BUILD"),
                this.GetChangeSetTemplate(package.Build.ChangeSet),
                this.GetChauffeurTemplate()
            };

            return templates;
        }

        #endregion
    }
}