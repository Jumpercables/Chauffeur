using System;

using Chauffeur.Jenkins.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Services
{
    [TestClass]
    public class ChauffeurServiceTest
    {
        [TestMethod]
        public void ChauffeurService_InstallLastSuccessfulBuild()
        {
            ChauffeurService service  = new ChauffeurService();
            var build = service.InstallLastSuccessfulBuild("Sempra_Release_9.3.1_SP2_HPDB");
        }
    }
}
