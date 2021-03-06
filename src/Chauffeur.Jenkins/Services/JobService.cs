﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Configuration;
using Chauffeur.Jenkins.Model;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     Provides a WFC service contract for obtaining job information from Jenkins.
    /// </summary>
    [ServiceContract]
    public interface IJobService
    {
        #region Public Methods

        /// <summary>
        ///     Gets the build with the specified build number for the job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build for the job.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/Build/{buildNumber}", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetBuildAsync(string jobName, string buildNumber);

        /// <summary>
        ///     Gets the first build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the first build.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/FirstBuild", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetFirstBuildAsync(string jobName);

        /// <summary>
        ///     Gets the job from server with the specified name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}", ResponseFormat = WebMessageFormat.Json)]
        Task<Job> GetJobAsync(string jobName);

        /// <summary>
        ///     Gets the last build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last build.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/LastBuild", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetLastBuildAsync(string jobName);

        /// <summary>
        ///     Gets the last completed build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last completed build.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/LastCompletedBuild", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetLastCompletedBuildAsync(string jobName);

        /// <summary>
        ///     Gets the last failed build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last failed build.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/LastFailedBuild", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetLastFailedBuildAsync(string jobName);

        /// <summary>
        ///     Gets the last stable build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last stable build.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/LastStableBuild", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetLastStableBuildAsync(string jobName);

        /// <summary>
        ///     Gets the last successful build for the job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last successful build for the job.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}/LastSuccessfulBuild", ResponseFormat = WebMessageFormat.Json)]
        Task<Build> GetLastSuccessfulBuildAsync(string jobName);

        /// <summary>
        ///     Gets the URI templates that are availabe for the service.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the methods available.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "?", ResponseFormat = WebMessageFormat.Json)]
        List<string> GetUriTemplates();

        #endregion
    }

    /// <summary>
    ///     Provides a WCF Service used to obtain the job information from jenkins.
    /// </summary>
    public class JobService : JenkinsService, IJobService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JobService" /> class.
        /// </summary>
        public JobService()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JobService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        internal JobService(Uri baseUri, JenkinsClient client, ChauffeurConfiguration configuration)
            : base(baseUri, client, configuration)
        {
        }

        #endregion

        #region IJobService Members

        /// <summary>
        ///     Gets the job from server with the specified name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        public Task<Job> GetJobAsync(string jobName)
        {
            return Task.Run(() =>
            {
                var job = this.Client.GetResource<Job>(base.BaseUri, "job", jobName);
                return job;
            });
        }

        /// <summary>
        ///     Gets the last successful build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last successful build for the job.
        /// </returns>
        /// <exception cref="WebFaultException{T}">
        ///     new ErrorData(The job name was not provided., The jobName argument cannot be null.)
        ///     or
        ///     new ErrorData(The job was not found., A job with the specified name does not exist.)
        /// </exception>
        /// <exception cref="ErrorData">
        ///     The job name was not provided.;The jobName argument cannot be null.
        ///     or
        ///     The job was not found.;A job with the specified name does not exist.
        /// </exception>
        public Task<Build> GetLastSuccessfulBuildAsync(string jobName)
        {
            return this.GetBuild(jobName, "lastSuccessfulBuild");
        }


        /// <summary>
        ///     Gets the build with the specified build number for the job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the build for the job.
        /// </returns>
        public Task<Build> GetBuildAsync(string jobName, string buildNumber)
        {
            return Task.Run(() =>
            {
                var build = this.Client.GetResource<Build>(base.BaseUri, "job", jobName, buildNumber);
                return build;
            });
        }

        /// <summary>
        ///     Gets the last stable build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last stable build.
        /// </returns>
        public Task<Build> GetLastStableBuildAsync(string jobName)
        {
            return this.GetBuild(jobName, "lastStableBuild");
        }

        /// <summary>
        ///     Gets the last build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last build.
        /// </returns>
        public Task<Build> GetLastBuildAsync(string jobName)
        {
            return this.GetBuild(jobName, "lastBuild");
        }

        /// <summary>
        ///     Gets the last completed build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last completed build.
        /// </returns>
        public Task<Build> GetLastCompletedBuildAsync(string jobName)
        {
            return this.GetBuild(jobName, "lastCompletedBuild");
        }

        /// <summary>
        ///     Gets the first build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the first build.
        /// </returns>
        public Task<Build> GetFirstBuildAsync(string jobName)
        {
            return this.GetBuild(jobName, "firstBuild");
        }

        /// <summary>
        ///     Gets the last failed build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last failed build.
        /// </returns>
        public Task<Build> GetLastFailedBuildAsync(string jobName)
        {
            return this.GetBuild(jobName, "lastFailedBuild");
        }

        /// <summary>
        ///     Gets the URI templates that are availabe for the service.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the methods available.
        /// </returns>
        public List<string> GetUriTemplates()
        {
            return this.GetUriTemplates(typeof (JobService));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the build for the specified build type.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildType">Type of the build.</param>
        /// <returns>Returns a <see cref="Build" /> representing the build for the build type.</returns>
        private Task<Build> GetBuild(string jobName, string buildType)
        {
            return Task.Run(() =>
            {
                var build = this.Client.GetResource<Build>(base.BaseUri, "job", jobName, buildType);
                return build;
            });
        }

        #endregion
    }
}