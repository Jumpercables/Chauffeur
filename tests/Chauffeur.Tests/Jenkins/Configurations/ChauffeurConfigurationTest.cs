using System;
using System.Diagnostics;
using System.IO;

using Chauffeur.Jenkins.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Configurations
{
    [TestClass]
    public class ChauffeurConfigurationTest
    {
        private ChauffeurConfiguration _Configuration;

        [TestInitialize]
        public void ChauffeurConfiguration_Initialize()
        {
            _Configuration = new ChauffeurConfiguration();
        }

        [TestMethod]
        public void ChauffeurConfiguration_ArtifactsDirectory()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");
            Assert.AreEqual(path,_Configuration.ArtifactsDirectory);
        }

        [TestMethod]
        public void ChauffeurConfiguration_BodyTemplateFile()
        {
            string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string path = Path.GetFullPath(Path.Combine(dir, "..\\Templates\\_Notification-Body.xslt"));
            Assert.AreEqual(path, _Configuration.BodyTemplateFile);
        }

        [TestMethod]
        public void ChauffeurConfiguration_DataDirectory()
        {
            string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string path = Path.GetFullPath(Path.Combine(dir, "..\\Data"));
            Assert.AreEqual(path, _Configuration.DataDirectory);
        }

        [TestMethod]
        public void ChauffeurConfiguration_From()
        {
            Assert.AreEqual("from@test.com", _Configuration.From);
        }

        [TestMethod]
        public void ChauffeurConfiguration_Host()
        {
            Assert.AreEqual("smtp.test.com", _Configuration.Host);
        }

        [TestMethod]
        public void ChauffeurConfiguration_InstallPropertyReferences()
        {
            Assert.AreEqual("COMPANY=A ENVIRONMENT=DEV", _Configuration.InstallPropertyReferences);
        }

        [TestMethod]
        public void ChauffeurConfiguration_PackageDataFile()
        {
            string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string path = Path.GetFullPath(Path.Combine(dir, "..\\Data\\Packages.json"));
            Assert.AreEqual(path, _Configuration.PackagesDataFile);
        }

        [TestMethod]
        public void ChauffeurConfiguration_Server()
        {
            Assert.AreEqual("http://localhost:8080/", _Configuration.Server);
        }

        [TestMethod]
        public void ChauffeurConfiguration_SubjectTemplateFile()
        {
            string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string path = Path.GetFullPath(Path.Combine(dir, "..\\Templates\\_Notification-Subject.xslt"));
            Assert.AreEqual(path, _Configuration.SubjectTemplateFile);
        }

        [TestMethod]
        public void ChauffeurConfiguration_TemplateDirectory()
        {
            string dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string path = Path.GetFullPath(Path.Combine(dir, "..\\Templates"));
            Assert.AreEqual(path, _Configuration.TemplateDirectory);
        }

        [TestMethod]
        public void ChauffeurConfiguration_To()
        {
            Assert.AreEqual("to@test.com", _Configuration.To);
        }

        [TestMethod]
        public void ChauffeurConfiguration_Token()
        {
            Assert.AreEqual("123131231231", _Configuration.Token);
        }

        [TestMethod]
        public void ChauffeurConfiguration_UninstallPropertyReferences()
        {
            Assert.AreEqual("ENVIRONMENT=DEV", _Configuration.UninstallPropertyReferences);
        }

        [TestMethod]
        public void ChauffeurConfiguration_User()
        {
            Assert.AreEqual("test", _Configuration.User);
        }
    }
}
