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
            var build = this.GetResource<Build>("build.valid.json");

            var service = new NotificationService();           
            var task = service.SendAsync(build);
            task.Wait();
        }

        #endregion
    }
}