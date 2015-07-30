using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Client;
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
        ///     Gets the last successful build for the job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>Returns a <see cref="Build" /> representing the last successful build for the job.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "Build/{jobName}")]
        Task<Build> GetBuildAsync(string jobName);

        /// <summary>
        ///     Gets the job from server with the specified name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Job/{jobName}")]
        Task<Job> GetJobAsync(string jobName);

        /// <summary>
        ///     Gets all of the jobs that have been configured in jenkins.
        /// </summary>
        /// <returns>Returns a <see cref="IList{Job}" /> representing jobs configured.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "Jobs")]
        Task<IList<Job>> GetJobsAsync();

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
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        internal JobService(Uri baseUri, JenkinsClient client)
            : base(baseUri, client)
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
        public Task<Job> GetJobAsync(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new WebFaultException<ErrorData>(new ErrorData("The job name was not provided.", "The jobName argument cannot be null."), HttpStatusCode.NotFound);

            try
            {
                return Task.Run(() =>
                {
                    this.Log("Job: {0}", jobName);

                    var queryUri = this.CreateUri(jobName);
                    var job = base.Client.GetResource<Job>(queryUri, 1);                   
                    return job;
                });
            }
            catch (WebException)
            {
                throw new WebFaultException<ErrorData>(new ErrorData("The job was not found.", "A job with the specified name does not exist."), HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        ///     Gets the last successful build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Build" /> representing the last successful build for the job.
        /// </returns>
        /// <exception cref="WebFaultException{ErrorData}">
        ///     new ErrorData(The job name was not provided., The jobName argument cannot be null.)
        ///     or
        ///     new ErrorData(The job was not found., A job with the specified name does not exist.)
        /// </exception>
        /// <exception cref="ErrorData">
        ///     The job name was not provided.;The jobName argument cannot be null.
        ///     or
        ///     The job was not found.;A job with the specified name does not exist.
        /// </exception>
        public Task<Build> GetBuildAsync(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new WebFaultException<ErrorData>(new ErrorData("The job name was not provided.", "The jobName argument cannot be null."), HttpStatusCode.NotFound);

            try
            {
                return Task.Run(() =>
                {
                    this.Log("Job: {0}", jobName);

                    var queryUri = this.CreateUri(jobName, "lastSuccessfulBuild");
                    var build = base.Client.GetResource<Build>(queryUri, 1);
                    
                    this.Log("Build: {0}", build.Number);

                    return build;
                });
            }
            catch (WebException)
            {
                throw new WebFaultException<ErrorData>(new ErrorData("The job was not found.", "A job with the specified name does not exist."), HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        ///     Gets all of the jobs that have been configured in jenkins.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="IList{Job}" /> representing all of the jobs.
        /// </returns>
        public Task<IList<Job>> GetJobsAsync()
        {
            return Task.Run(() =>
            {
                var tree = base.Client.GetResource<Tree>(base.BaseUri, 1);
                return tree != null ? tree.Jobs : null;
            });
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the URI for the specific job.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>
        ///     Returns a <see cref="Uri" /> representing the query string for the job.
        /// </returns>
        protected Uri CreateUri(string jobName, params string[] queryString)
        {
            var queryUri = new Uri(base.BaseUri, @"/job/" + jobName + "/" + string.Join("/", queryString) + "/");
            return queryUri;
        }

        #endregion
    }
}