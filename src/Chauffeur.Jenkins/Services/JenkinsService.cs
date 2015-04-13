using System;
using System.Configuration;
using System.Diagnostics;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     Provides an abstract jenkins service for the client.
    /// </summary>
    public abstract class JenkinsService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JenkinsService" /> class.
        /// </summary>
        protected JenkinsService()
        {
            string url = ConfigurationManager.AppSettings["server"];
            this.BaseUri = new Uri(url);

            string user = ConfigurationManager.AppSettings["user"];
            string token = ConfigurationManager.AppSettings["token"];
            this.Client = new JsonJenkinsClient(user, token);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JenkinsService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        protected JenkinsService(Uri baseUri, JenkinsClient client)
        {
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");

            if (client == null)
                throw new ArgumentNullException("client");

            this.BaseUri = baseUri;
            this.Client = client;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the base URI.
        /// </summary>
        /// <value>
        ///     The base URI.
        /// </value>
        protected Uri BaseUri { get; set; }

        /// <summary>
        ///     Gets or sets the client.
        /// </summary>
        /// <value>
        ///     The client.
        /// </value>
        protected JenkinsClient Client { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Logs the message in the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        protected void Log(string format, params object[] args)
        {
            Trace.WriteLine(string.Format("{0} - {1}", DateTime.Now, string.Format(format, args)));
        }

        #endregion
    }
}