using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Model;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chauffeur.Jenkins.Services;

namespace Chauffeur.Tests.Jenkins.Services
{
    [TestClass]
    public class NotificationServiceTest : JenkinsClientTest
    {
        [TestMethod]
        public void NotificationService_Send()
        {
            var package = this.GetResource<Package>("package.valid.json");
            NotificationService service = new NotificationService();
            var task = service.SendAsync(package);
            task.Wait();

            Assert.AreEqual(task.Result, true);
        }
    }
}
