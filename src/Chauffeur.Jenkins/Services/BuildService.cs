using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Text;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Services
{
    [ServiceContract]
    public interface IBuildService
    {
        #region Public Methods

        /// <summary>
        /// Initiates the build for the specified job name with optional build parameters.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="cause">The cause.</param>
        /// <param name="parameters">The optional build parameters.</param>
        [OperationContract]
        void Build(string jobName, string cause, Dictionary<string, string> parameters);

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class BuildService : JenkinsService, IBuildService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildService" /> class.
        /// </summary>
        public BuildService()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        internal BuildService(Uri baseUri, JenkinsClient client)
            : base(baseUri, client)
        {
        }

        #endregion

        #region IBuildService Members

        /// <summary>
        /// Initiates the build for the specified job name with optional build parameters.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="cause">The cause.</param>
        /// <param name="parameters">The optional build parameters.</param>
        /// <exception cref="System.ServiceModel.FaultException">The job was not found.</exception>
        public void Build(string jobName, string cause, Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                var uri = new Uri(base.BaseUri, "build?token=" + base.Client.ApiToken);
                base.Client.Post(uri);
            }
            else
            {
                StringBuilder sb = new StringBuilder(parameters.Count);

                foreach (var parameter in parameters)
                {
                    sb.Append("&");
                    sb.Append(parameter.Key);
                    sb.Append("=");
                    sb.Append(parameter.Value);
                }

                if (!string.IsNullOrEmpty(cause))
                {
                    sb.Append("&cause=");
                    sb.Append(cause);
                }

                try
                {
                    var uri = new Uri(base.BaseUri, "buildWithParameters?token=" + base.Client.ApiToken + sb);
                    base.Client.Post(uri);
                }
                catch (WebException)
                {
                    throw new FaultException("The job was not found.");
                }
            }
        }



        #endregion
    }
}