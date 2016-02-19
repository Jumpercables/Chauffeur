using System;
using System.Net;
using System.Text;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    ///     The jenkins resposne object.
    /// </summary>
    public interface IUrl
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        Uri Url { get; set; }

        #endregion
    }

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
        ///     Gets the resource by requesting it from the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="queries">The query string.</param>
        /// <returns>
        ///     Returns a <see cref="T" /> representing the resource.
        /// </returns>
        public T GetResource<T>(Uri resourceUri, params string[] queries) where T : class, IUrl
        {
            var queryUri = new Uri(resourceUri, string.Join("/", queries) + "/");
            return this.GetResource<T>(queryUri, 1);
        }

        /// <summary>
        ///     Retrieves a jpenkins resources given it's URI.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="func">The function delegate that converts the response into the type..</param>
        /// <returns>Returns a <see cref="T" /> representing the resource converted to the type.</returns>
        public T GetResource<T>(Uri resourceUri, Func<WebResponse, T> func)
        {
            var request = Create(resourceUri, WebRequestMethods.Http.Get);
            return func(request.GetResponse());
        }

        /// <summary>
        ///     Retrieves a jenkins resource given it's URI and optional tree parameter.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resourceUri">The absolute URI of that resource (not including the api suffix).</param>
        /// <param name="tree">
        ///     A tree parameter, which will filter what properties are selected. See the Jenkins API documentation
        ///     for details.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="T" /> representing the resource converted to the type.
        /// </returns>
        public T GetResource<T>(Uri resourceUri, string tree) where T : class, IUrl
        {
            var absoluteUri = this.GetAbsoluteUri(resourceUri, tree);
            return this.GetResource<T>(absoluteUri, this.GetResource<T>);
        }

        /// <summary>
        ///     Retrieves a jenkins resource given it's URI and optional tree parameter.
        /// </summary>
        /// <typeparam name="T">The resource type.</typeparam>
        /// <param name="resourceUri">The absolute URI of that resource (not including the api suffix).</param>
        /// <param name="depth">The number of levels to select.  See the Jenkins API documentation for details.</param>
        /// <returns>
        ///     Returns a <see cref="T" /> representing the resource converted to the type.
        /// </returns>
        public T GetResource<T>(Uri resourceUri, int depth) where T : class, IUrl
        {
            var absoluteUri = this.GetAbsoluteUri(resourceUri, depth);
            return this.GetResource<T>(absoluteUri, this.GetResource<T>);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the resource that has been converted to the proper format for the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns>
        ///     Returns <see cref="T" /> representing the format for the client.
        /// </returns>
        protected abstract T GetResource<T>(WebResponse response);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the request.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">requestUri</exception>
        private WebRequest Create(Uri requestUri, string method)
        {
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");

            var request = WebRequest.Create(requestUri);
            request.Method = method;

            if (!string.IsNullOrEmpty(_UserName))
            {
                request.PreAuthenticate = true;

                var authInfo = _UserName + ":" + _ApiToken;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                request.Headers["Authorization"] = "Basic " + authInfo;
            }

            Log.Info(this, "{1}: {0}", requestUri.ToString(), request.Method);

            return request;
        }

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