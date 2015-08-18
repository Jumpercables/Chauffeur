using System;
using System.Linq;

using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Templates;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Model
{
    [TestClass]
    public class PackageTest : JenkinsClientTest
    {
        #region Public Methods

        [TestMethod]
        public void PackageApplyBodyXslTemplate()
        {
            var package = this.GetResource<Package>("package.valid.json");
            Assert.IsNotNull(package);

            var template = new StyleSheetTemplate<Package>(Configuration.BodyXslFile);
            var data = template.ApplyTemplate(package);
        }

        [TestMethod]
        public void PackageApplySubjectXslTemplate()
        {
            var package = this.GetResource<Package>("package.valid.json");
            Assert.IsNotNull(package);

            var template = new StyleSheetTemplate<Package>(Configuration.SubjectXslFile);
            var data = template.ApplyTemplate(package);
        }


        [TestMethod]
        public void PackageValidJson()
        {
            var package = this.GetResource<Package>("package.valid.json");
            Assert.IsNotNull(package);

            Assert.AreEqual(new Uri("file:///C:/ProgramData/Jenkins/Packages.json"), package.Url);
            Assert.AreEqual("8/12/2015", package.Date);
            Assert.AreEqual("Chauffer-Nightly-Build", package.Job);
            Assert.AreEqual(1, package.Paths.Length);
            Assert.AreEqual(@"C:\ProgramData\Jenkins\Chauffer-Nightly-Build.34.msi", package.Paths.First());

            BuildTest.ValidateBuild(package.Build);
        }

        #endregion
    }
}