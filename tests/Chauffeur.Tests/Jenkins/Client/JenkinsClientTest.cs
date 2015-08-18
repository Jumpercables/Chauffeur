using System;
using System.Reflection;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Client
{
    [TestClass]
    public abstract class JenkinsClientTest
    {
        #region Fields

        /// <summary>
        ///     The client
        /// </summary>
        protected static JsonJenkinsClient Client;

        /// <summary>
        ///     The configuration
        /// </summary>
        protected static ChauffeurConfiguration Configuration;

        /// <summary>
        ///     The web request factory
        /// </summary>
        protected static MockWebRequestFactory WebRequestFactory;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initializes the specified test context.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Client = new JsonJenkinsClient();
            WebRequestFactory = new MockWebRequestFactory("mock");
            Configuration = new ChauffeurConfiguration();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Loads the specified resource name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        ///     Returns the resource.
        /// </returns>
        protected T GetResource<T>(string resourceName) where T : class, IUrl
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = "Chauffeur.Tests.Json." + resourceName;
            var address = "mock://test/" + resourceName + "/api/json";

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                WebRequestFactory.Expect(new Uri(address), stream);
            }

            var resourceUri = new Uri("mock://test/" + resourceName + "/");
            return Client.GetResource<T>(resourceUri, 0);
        }

        #endregion
    }
}