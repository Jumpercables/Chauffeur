using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Chauffeur.Tests.Jenkins.Client
{
    public class MockWebRequest : WebRequest
    {
        #region Fields

        private readonly MemoryStream _MemoryStream = new MemoryStream();
        private readonly Uri _RequestUri;
        private readonly WebResponse _Response;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockWebRequest" /> class.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="response">The response.</param>
        public MockWebRequest(Uri requestUri, WebResponse response)
        {
            _RequestUri = requestUri;
            _Response = response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     When overridden in a descendant class, gets or sets the content length of the request data being sent.
        /// </summary>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override long ContentLength { get; set; }

        /// <summary>
        ///     When overridden in a descendant class, gets or sets the content type of the request data being sent.
        /// </summary>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override string ContentType { get; set; }

        /// <summary>
        ///     When overridden in a descendant class, gets or sets the protocol method to use in this request.
        /// </summary>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override string Method { get; set; }

        /// <summary>
        ///     When overridden in a descendant class, gets the URI of the Internet resource associated with the request.
        /// </summary>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override Uri RequestUri
        {
            get { return _RequestUri; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     When overridden in a descendant class, begins an asynchronous request for an Internet resource.
        /// </summary>
        /// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
        /// <param name="state">An object containing state information for this asynchronous request.</param>
        /// <returns>
        ///     An <see cref="T:System.IAsyncResult" /> that references the asynchronous request.
        /// </returns>
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return this.GetResponseTask();
        }

        /// <summary>
        ///     When overridden in a descendant class, returns a <see cref="T:System.Net.WebResponse" />.
        /// </summary>
        /// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that references a pending request for a response.</param>
        /// <returns>
        ///     A <see cref="T:System.Net.WebResponse" /> that contains a response to the Internet request.
        /// </returns>
        public override WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            var task = (Task<WebResponse>) asyncResult;
            task.Wait();
            return task.Result;
        }

        /// <summary>
        ///     When overridden in a descendant class, returns a <see cref="T:System.IO.Stream" /> for writing data to the Internet
        ///     resource.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.IO.Stream" /> for writing data to the Internet resource.
        /// </returns>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override Stream GetRequestStream()
        {
            return _MemoryStream;
        }

        /// <summary>
        ///     When overridden in a descendant class, returns a response to an Internet request.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Net.WebResponse" /> containing the response to the Internet request.
        /// </returns>
        /// <exception cref="System.Net.WebException">Mock URI not setup:  + _RequestUri</exception>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override WebResponse GetResponse()
        {
            return _Response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the response task.
        /// </summary>
        /// <returns></returns>
        private Task<WebResponse> GetResponseTask()
        {
            return Task.Factory.StartNew<WebResponse>(GetResponse);
        }

        #endregion
    }
}