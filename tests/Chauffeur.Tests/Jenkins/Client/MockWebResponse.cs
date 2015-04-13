using System.IO;
using System.Net;
using System.Text;

namespace Chauffeur.Tests.Jenkins.Client
{
    internal class MockWebResponse : WebResponse
    {
        #region Fields

        private readonly string _Response;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockWebResponse" /> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public MockWebResponse(string response)
        {
            _Response = response;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     When overridden in a descendant class, returns the data stream from the Internet resource.
        /// </summary>
        /// <returns>
        ///     An instance of the <see cref="T:System.IO.Stream" /> class for reading data from the Internet resource.
        /// </returns>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public override Stream GetResponseStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_Response));
        }

        #endregion
    }
}