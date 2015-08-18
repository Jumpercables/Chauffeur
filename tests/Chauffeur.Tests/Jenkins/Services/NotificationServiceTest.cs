using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Services
{
    [TestClass]
    public class NotificationServiceTest : JenkinsClientTest
    {
        #region Public Methods

        [TestMethod]
        public void NotificationServiceSendAsyncWithPackage()
        {
            var package = this.GetResource<Package>("package.valid.json");
            var service = new NotificationService();
            var task = service.SendAsync(package);
            task.Wait();

            Assert.IsTrue(task.Result);
        }

        #endregion
    }
}