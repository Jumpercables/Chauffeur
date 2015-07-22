using System;

using Chauffeur.Jenkins.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Services
{
    [TestClass]
    public class ChauffeurServiceTest
    {
        [TestMethod]
        public async void ChauffeurService_InstallBuildAsync()
        {
            ChauffeurService service  = new ChauffeurService();
            var build = await service.InstallBuildAsync("Sempra_Release_9.3.1_SP2_HPDB");
        }
    }
}
