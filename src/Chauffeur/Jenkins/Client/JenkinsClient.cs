using System;
using System.Net;

namespace Chauffeur.Jenkins.Client
{
    /// <summary>
    ///     An abstract client used to handle reading the Jenkins REST API.
    /// </summary>
    public abstract class JenkinsClient
    {
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
        /// <returns>Returns the <see cref="WebRequest" /> for the URI.</returns>
        public abstract WebRequest GetRequest(Uri absoluteUri);

        /// <summary>
        ///     Retrieves a jenkins resource given it's URI and optional tree parameter.
        /// </summary>
        /// <param name="resourceUri">The absolute URI of that resource (not including the api suffix).</param>
        /// <param name="tree">
        ///     A tree parameter, which will filter what properties are selected. See the Jenkins API documentation for details.
        /// </param>
        public T GetResource<T>(Uri resourceUri, string tree) where T : class, IUrl
        {
            var absoluteUri = GetAbsoluteUri(resourceUri, tree);
            var request = GetRequest(absoluteUri);
            return GetResource<T>(request);
        }

        /// <summary>
        ///     Retrieves a jenkins resource given it's URI and optional tree parameter.
        /// </summary>
        /// <param name="resourceUri">The absolute URI of that resource (not including the api suffix).</param>
        /// <param name="depth">
        ///     The number of levels to select.  See the Jenkins API documentation for details.
        /// </param>
        public T GetResource<T>(Uri resourceUri, int depth) where T : class, IUrl
        {
            var absoluteUri = GetAbsoluteUri(resourceUri, depth);
            var request = GetRequest(absoluteUri);
            return GetResource<T>(request);
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