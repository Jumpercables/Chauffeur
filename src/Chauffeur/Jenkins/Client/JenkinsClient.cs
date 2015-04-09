using System;
using System.Net;
using System.Text;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    ///     An abstract client used to handle reading the Jenkins REST API.
    /// </summary>
    public abstract class JenkinsClient
    {
        #region Fields

        private readonly string _ApiToken;
        private readonly string _UserName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JenkinsClient" /> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="apiToken">The API token.</param>
        protected JenkinsClient(string userName, string apiToken)
        {
            _UserName = userName;
            _ApiToken = apiToken;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        ///     Gets the API token.
        /// </summary>
        /// <value>
        ///     The API token.
        /// </value>
        internal string ApiToken
        {
            get { return _ApiToken; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the API suffix.
        /// </summary>
        /// <value>
        ///     The API suffix.
        /// </value>
        protected abstract string ApiSuffix { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Expands a partially retrieved resource, with an optional tree parameter.
        /// </summary>
        /// <remarks>
        ///     It's important to notice this method returns a new instance of the resource, and doesn't
        ///     change the passed in instance.
        /// </remarks>
        /// <param name="resource">A previously retrieved instance of this resource.</param>
        /// <param name="tree">
        ///     A tree parameter, which will filter what properties are selected. See the Jenkins API documentation for details.
        /// </param>
        public T Expand<T>(T resource, string tree = null) where T : class, IUrl
        {
            return resource == null ? null : this.GetResource<T>(resource.Url, tree);
        }

        /// <summary>
        ///     Expands a partially retrieved resource, with an optional tree parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">A previously retrieved instance of this resource.</param>
        /// <param name="depth">The number of levels to select.  See the Jenkins API documentation for details.</param>
        /// <returns></returns>
        /// <remarks>
        ///     It's important to notice this method returns a new instance of the resource, and doesn't
        ///     change the passed in instance.
        /// </remarks>
        public T Expand<T>(T resource, int depth) where T : class, IUrl
        {
            return resource == null ? null : this.GetResource<T>(resource.Url, depth);
        }

        /// <summary>
        ///     Gets the request.
        /// </summary>
        /// <param name="absoluteUri">The absolute URI.</param>
        /// <returns>
        ///     Returns the <see cref="WebRequest" /> for the URI.
        /// </returns>
        public virtual WebRequest GetRequest(Uri absoluteUri)
        {
            var request = WebRequest.Create(absoluteUri);
            request.Method = "GET";

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
        /// Retrieves a jenkins resource given it's URI and optional tree parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceUri">The absolute URI of that resource (not including the api suffix).</param>
        /// <param name="tree">A tree parameter, which will filter what properties are selected. See the Jenkins API documentation
        /// for details.</param>
        /// <returns>
        /// Returns a <see cref="T" /> representing the resource converted to the type.
        /// </returns>
        public T GetResource<T>(Uri resourceUri, string tree) where T : class, IUrl
        {
            var absoluteUri = this.GetAbsoluteUri(resourceUri, tree);
            var request = this.GetRequest(absoluteUri);
            return this.GetResource<T>(request);
        }

        /// <summary>
        ///     Retrieves a jenkins resource given it's URI and optional tree parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceUri">The absolute URI of that resource (not including the api suffix).</param>
        /// <param name="depth">The number of levels to select.  See the Jenkins API documentation for details.</param>
        /// <returns>
        ///     Returns a <see cref="T" /> representing the resource converted to the type.
        /// </returns>
        public T GetResource<T>(Uri resourceUri, int depth) where T : class, IUrl
        {
            var absoluteUri = this.GetAbsoluteUri(resourceUri, depth);
            var request = this.GetRequest(absoluteUri);
            return this.GetResource<T>(request);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the resource that has been converted to the proper format for the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns <see cref="T" /> representing the format for the client.
        /// </returns>
        protected abstract T GetResource<T>(WebRequest request);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the absolute URI.
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="tree">The tree.</param>
        /// <returns>
        ///     Returns a <see cref="Uri" /> representing the URL.
        /// </returns>
        private Uri GetAbsoluteUri(Uri resourceUri, string tree)
        {
            var relativeUri = this.ApiSuffix;
            if (!string.IsNullOrWhiteSpace(tree))
                relativeUri += "?tree=" + tree;

            var absoluteUri = new Uri(resourceUri, relativeUri);
            return absoluteUri;
        }

        /// <summary>
        ///     Gets the absolute URI.
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>
        ///     Returns a <see cref="Uri" /> representing the URL.
        /// </returns>
        private Uri GetAbsoluteUri(Uri resourceUri, int depth)
        {
            var relativeUri = this.ApiSuffix;
            if (depth > 0)
                relativeUri += "?depth=" + depth;

            var absoluteUri = new Uri(resourceUri, relativeUri);
            return absoluteUri;
        }

        #endregion
    }
}