using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Configuration;

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
            : this(new ChauffeurConfiguration())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JenkinsService" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected JenkinsService(ChauffeurConfiguration configuration)
            : this(new Uri(configuration.Jenkins.Server), new JsonJenkinsClient(configuration.Jenkins.User, configuration.Jenkins.Token), configuration)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JenkinsService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        protected JenkinsService(Uri baseUri, JenkinsClient client, ChauffeurConfiguration configuration)
        {
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");

            if (client == null)
                throw new ArgumentNullException("client");

            this.BaseUri = baseUri;
            this.Client = client;
            this.Configuration = configuration;
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

        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        protected ChauffeurConfiguration Configuration { get; set; }

        #endregion
       
        #region Protected Methods

        /// <summary>
        ///     Gets the URI templates that are availabe for the service.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the methods available.
        /// </returns>
        protected List<string> GetUriTemplates(Type service)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

            List<string> list = new List<string>();

            foreach (var contract in service.GetInterfaces())
            {
                var methods = contract.GetMethods().Where(method => method.GetCustomAttributes(typeof (OperationContractAttribute), false).Any() && method.Name != methodBase.Name);

                foreach (var attribute in methods.Select(method => method.GetCustomAttribute<WebGetAttribute>()))
                {
                    list.Add(attribute.UriTemplate);
                }
            }

            return list;
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
            this.Details = detailedInformation;
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
        public string Details { get; private set; }

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