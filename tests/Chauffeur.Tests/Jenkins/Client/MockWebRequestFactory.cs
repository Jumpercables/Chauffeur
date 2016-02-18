using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Net;

namespace Chauffeur.Tests.Jenkins.Client
{
    public class MockWebRequestFactory : IWebRequestCreate
    {
        #region Fields

        private readonly ConcurrentDictionary<Uri, MockWebResponse> _Responses = new ConcurrentDictionary<Uri, MockWebResponse>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockWebRequestFactory" /> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public MockWebRequestFactory(string prefix)
        {
            WebRequest.RegisterPrefix(prefix, this);
        }

        #endregion

        #region IWebRequestCreate Members

        /// <summary>
        ///     Creates a <see cref="T:System.Net.WebRequest" /> instance.
        /// </summary>
        /// <param name="uri">The uniform resource identifier (URI) of the Web resource.</param>
        /// <returns>
        ///     A <see cref="T:System.Net.WebRequest" /> instance.
        /// </returns>
        public WebRequest Create(Uri uri)
        {
            MockWebResponse response;

            _Responses.TryGetValue(uri, out response);
            return new MockWebRequest(uri, response);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Expects the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="response">The response.</param>
        internal void Expect(Uri uri, MockWebResponse response)
        {
            _Responses[uri] = response;
        }

        /// <summary>
        ///     Expects the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="response">The response.</param>
        internal void Expect(Uri uri, string response)
        {
            this.Expect(uri, new MockWebResponse(response));
        }

        /// <summary>
        ///     Expects the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="stream">The stream.</param>
        internal void Expect(Uri uri, Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                this.Expect(uri, streamReader.ReadToEnd());
            }
        }

        #endregion
    }
}