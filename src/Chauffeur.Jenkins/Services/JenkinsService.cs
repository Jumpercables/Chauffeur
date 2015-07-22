using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Web;

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
            string url = ConfigurationManager.AppSettings["jenkins.server"];

            if (string.IsNullOrEmpty(url))
                throw new WebFaultException<ErrorData>(new ErrorData("The 'server' must be provided.", "The 'jenkins.server' must be provided in the configuration file."), HttpStatusCode.NotFound);


            string user = ConfigurationManager.AppSettings["jenkins.user"];
            if (string.IsNullOrEmpty(user))
                throw new WebFaultException<ErrorData>(new ErrorData("The 'user' must be provided.", "The 'jenkins.user' must be provided in the configuration file."), HttpStatusCode.NotFound);

            string token = ConfigurationManager.AppSettings["jenkins.token"];
            if (string.IsNullOrEmpty(token))
                throw new WebFaultException<ErrorData>(new ErrorData("The 'token' must be provided.", "The 'jenkins.token' must be provided in the configuration file."), HttpStatusCode.NotFound);

            this.BaseUri = new Uri(url);
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

    /// <summary>
    ///     A light weight structure used for the details in the WebFaultException.
    /// </summary>
    [DataContract]
    public class ErrorData
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorData" /> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="detailedInformation">The detailed information.</param>
        public ErrorData(string reason, string detailedInformation)
        {
            this.Reason = reason;
            this.DetailedInformation = detailedInformation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the detailed information.
        /// </summary>
        /// <value>
        ///     The detailed information.
        /// </value>
        [DataMember]
        public string DetailedInformation { get; private set; }

        /// <summary>
        ///     Gets the reason.
        /// </summary>
        /// <value>
        ///     The reason.
        /// </value>
        [DataMember]
        public string Reason { get; private set; }

        #endregion
    }
}