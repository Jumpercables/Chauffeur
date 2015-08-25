using System;
using System.Linq;

using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services.Templates;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Model
{
    [TestClass]
    public class PackageTest : JenkinsClientTest
    {
        #region Public Methods

        [TestMethod]
        public void Package_ApplyBodyTemplate()
        {
            var package = this.GetResource<Package>("package.valid.json");
            Assert.IsNotNull(package);

            var template = new StyleSheetTemplate<Package>(Configuration.BodyTemplateFile);
            var data = template.ApplyTemplate(package);
        }

        [TestMethod]
        public void Package_ApplySubjectTemplate()
        {
            var package = this.GetResource<Package>("package.valid.json");
            Assert.IsNotNull(package);

            var template = new StyleSheetTemplate<Package>(Configuration.SubjectTemplateFile);
            var data = template.ApplyTemplate(package);
            Assert.AreEqual("Build 34 Installed.", data);
        }


        [TestMethod]
        public void Package_Json()
        {
            var package = this.GetResource<Package>("package.valid.json");
            Assert.IsNotNull(package);

            Assert.AreEqual(new Uri("file:///C:/ProgramData/Jenkins/Packages.json"), package.Url);
            Assert.AreEqual("8/12/2015", package.Date);
            Assert.AreEqual("Chauffer-Nightly-Build", package.Job);
            Assert.AreEqual(1, package.Paths.Length);
            Assert.AreEqual(@"C:\ProgramData\Jenkins\Chauffer-Nightly-Build.34.msi", package.Paths.First());
        }

        #endregion
    }
}