using System;

using Chauffeur.Jenkins.Model;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Model
{
    [TestClass]
    public class BuildTest : JenkinsClientTest
    {
        #region Public Methods

        [TestMethod]
        public void Build_Validate()
        {
            var build = this.GetResource<Build>("build.valid.json");

            ValidateBuild(build);
        }

        public static void ValidateBuild(Build build)
        {
            Assert.IsNotNull(build);
            Assert.AreEqual(build.Building, false);
            Assert.AreEqual(build.BuiltOn, "");
            Assert.AreEqual(build.Description, null);
            Assert.AreEqual(build.DisplayName, "#34");
            Assert.AreEqual(build.Duration, 368737);
            Assert.AreEqual(build.FullDisplayName, "Chauffer-Nightly-Build #34");
            Assert.AreEqual(build.Id, "34");
            Assert.AreEqual(build.KeepLog, false);
            Assert.AreEqual(build.Number, 34);
            Assert.AreEqual(build.Result, "SUCCESS");
            Assert.AreEqual(build.Url, new Uri("http://localhost:8080/job/Chauffer-Nightly-Build/34/"));
        }

        #endregion
    }
}