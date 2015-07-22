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
        public async void NotificationService_SendAsync()
        {
            var build = this.GetResource<Build>("build.valid.json");
            
            NotificationService service = new NotificationService();
            bool result = await service.SendAsync(build);
        }

        #endregion
    }
}