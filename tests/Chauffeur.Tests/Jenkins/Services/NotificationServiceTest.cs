using System.Threading.Tasks;

using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Services
{
    [TestClass]
    public class NotificationServiceTest : JenkinsServiceTest
    {
        #region Public Methods

        [TestMethod]
        public void NotificationService_SendAsync()
        {
            var package = this.GetResource<Package>("package.valid.json");
            var service = new NotificationService();           
            var task = service.SendAsync(package);
            task.Wait();
        }

        #endregion
    }
}