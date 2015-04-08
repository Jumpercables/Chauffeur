using System;
using System.Net;
using System.Text;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    ///     A jenkins client that allows for authentication to the server.
    /// </summary>
    public abstract class AuthenticatedJenkinsClient : JenkinsClient
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
        protected AuthenticatedJenkinsClient(string userName, string apiToken)
        {
            _UserName = userName;
            _ApiToken = apiToken;
        }

        #endregion

        #region Public Methods

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

        #endregion
    }
}