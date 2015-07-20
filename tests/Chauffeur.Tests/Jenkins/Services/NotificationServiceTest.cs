using System.Threading.Tasks;

using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Services
{
    [TestClass]
    public class NotificationServiceTest
    {
        #region Public Methods

        [TestMethod]
        public void NotificationService_SendAsync()
        {
            NotificationService service = new NotificationService();
            bool result = service.Send(new Build() { Number = 123 });
        }

        #endregion
    }
}