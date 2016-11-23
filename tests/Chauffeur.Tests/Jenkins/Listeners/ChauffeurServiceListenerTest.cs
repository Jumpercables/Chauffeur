using Chauffeur.Jenkins.Listeners;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Listeners
{
    [TestClass]
    public class ChauffeurServiceListenerTest
    {
        #region Public Methods

        [TestMethod]
        public void ChauffeurServiceListener_Start()
        {
            ChaufferServiceListener listener = new ChaufferServiceListener("http://localhost:8733/Design_Time_Addresses/Chauffeur.Jenkins.Services/ChauffeurService/rest/");
            listener.Run((request) =>
            {
                Assert.AreEqual(request.Url, "http://localhost:8733/Design_Time_Addresses/Chauffeur.Jenkins.Services/ChauffeurService/mex");            
            });

            listener.Stop();
        }

        #endregion
    }
}