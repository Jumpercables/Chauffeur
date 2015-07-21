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
        public void NotificationService_Send()
        {
            var build = this.GetResource<Build>("build.valid.json");
            
            NotificationService service = new NotificationService();
            bool result = service.Send(build);
        }

        #endregion
    }
}