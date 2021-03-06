﻿using System.IO;
using System.Net;

using Newtonsoft.Json;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    ///     A jenkins client that assumes the response will be in the JSON format.
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
        ///     Gets the resource that has been converted to the proper format for the client.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="response">The response.</param>
        /// <returns>
        ///     Returns <see cref="T" /> representing the format for the client.
        /// </returns>
        protected override T GetResource<T>(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                if (stream == null) return default(T);

                using (JsonTextReader reader = new JsonTextReader(new StreamReader(stream)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                }
            }
        }

        #endregion
    }
}