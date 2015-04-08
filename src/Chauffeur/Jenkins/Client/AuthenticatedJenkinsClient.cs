using System;
using System.Net;
using System.Text;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    /// </summary>
    public class AuthenticatedJenkinsClient : JenkinsClient
    {
        #region Fields

        private readonly string _ApiToken;
        private readonly string _UserName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthenticatedJenkinsClient" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="apiToken">The API token.</param>
        public AuthenticatedJenkinsClient(string userName, string apiToken)
        {
            _UserName = userName;
            _ApiToken = apiToken;
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
            get { return null; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the authenticated request object.
        /// </summary>
        /// <param name="absoluteUri">The absolute URI.</param>
        /// <returns>Returns the <see cref="WebRequest" /> for the URI.</returns>
        public override WebRequest GetRequest(Uri absoluteUri)
        {
            var request = WebRequest.Create(absoluteUri);
            request.Method = "GET";
            request.Timeout = 90000;

            if (!string.IsNullOrEmpty(_UserName))
            {
                request.PreAuthenticate = true;

                var authInfo = _UserName + ":" + _ApiToken;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                request.Headers["Authorization"] = "Basic " + authInfo;
            }

            return request;
        }

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
            return default(T);
        }

        #endregion
    }
}