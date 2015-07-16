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
            var task = service.SendAsync(new Build());
            Task.WaitAll(task);
        }

        #endregion
    }
}