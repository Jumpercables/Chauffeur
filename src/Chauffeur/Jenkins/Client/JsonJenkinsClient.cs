using System;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    /// A jenkins client that assumes the response will be in the JSON format.
    /// </summary>
    public class JsonJenkinsClient : JenkinsClient
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonJenkinsClient" /> class.
        /// </summary>
        public JsonJenkinsClient()
            : base(null, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonJenkinsClient" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="apiToken">The API token.</param>
        public JsonJenkinsClient(string userName, string apiToken)
            : base(userName, apiToken)
        {
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the API suffix.
        /// </summary>
        /// <value>
        ///     The API suffix.
        /// </value>
        protected override string ApiSuffix
        {
            get { return "api/json"; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the resource that has been converted to the proper format for the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Returns <see cref="T" /> representing the format for the client.
        /// </returns>
        protected override T GetResource<T>(WebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) return default(T);

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(stream);
                }
            }
        }

        #endregion
    }
}