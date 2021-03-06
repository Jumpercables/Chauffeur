﻿using System;

using Chauffeur.Jenkins.Model;
using Chauffeur.Tests.Jenkins.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chauffeur.Tests.Jenkins.Model
{
    [TestClass]
    public class JobTest : JenkinsClientTest
    {
        #region Public Methods

        [TestMethod]
        public void Job_Json()
        {
            var job = this.GetResource<Job>("job.valid.json");

            Assert.IsNotNull(job);
            Assert.AreEqual("The nightly build of the chauffuer test client.", job.Description);
            Assert.AreEqual("Chauffer-Nightly-Build", job.DisplayName);
            Assert.AreEqual("Chauffer-Nightly-Build", job.Name);
            Assert.AreEqual(new Uri("http://localhost:8080/view/job/Chauffer-Nightly-Build/"), job.Url);
            Assert.AreEqual(true, job.Buildable);

            Assert.AreEqual(25, job.Builds.Count);
            Assert.AreEqual(2208, job.Builds[3].Number);
            Assert.AreEqual(new Uri("http://localhost:8080/view/job/Chauffer-Nightly-Build/2211/"), job.Builds[0].Url);
            Assert.AreEqual("blue", job.Color);
            Assert.AreEqual(2187, job.FirstBuild.Number);

            Assert.AreEqual(1, job.HealthReports.Count);
            Assert.AreEqual("Build stability: 2 out of the last 5 builds failed.", job.HealthReports[0].Description);
            Assert.AreEqual("health-40to59.png", job.HealthReports[0].IconUrl);
            Assert.AreEqual(60, job.HealthReports[0].Score);

            Assert.AreEqual(false, job.InQueue);
            Assert.AreEqual(false, job.KeepDependencies);
            Assert.AreEqual(2211, job.LastBuild.Number);
            Assert.AreEqual(2211, job.LastCompletedBuild.Number);
            Assert.AreEqual(2208, job.LastFailedBuild.Number);
            Assert.AreEqual(2211, job.LastStableBuild.Number);
            Assert.AreEqual(2211, job.LastSuccessfulBuild.Number);
            Assert.IsNull(job.LastUnstableBuild);
            Assert.AreEqual(2208, job.LastUnsuccessfulBuild.Number);
            Assert.AreEqual(2212, job.NextBuildNumber);
            Assert.AreEqual(false, job.ConcurrentBuild);
            Assert.AreEqual(0, job.DownstreamProjects.Count);
            Assert.AreEqual(0, job.UpstreamProjects.Count);
        }

        #endregion
    }
}