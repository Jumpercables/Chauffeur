using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;

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
        ///     Gets the job from server with the specified name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        [OperationContract]
        Job GetJob(string jobName);

        /// <summary>
        ///     Gets all of the jobs that have been configured in jenkins.
        /// </summary>
        /// <returns>Returns a <see cref="IList{Job}" /> representing jobs configured.</returns>
        [OperationContract]
        IList<Job> GetJobs();

        #endregion
    }

    /// <summary>
    /// Provides a WCF Service used to obtain the job information from jenkins.
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
        /// Gets the job from server with the specified name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        /// Returns a <see cref="Job" /> representing the job.
        /// </returns>
        /// <exception cref="System.ServiceModel.FaultException">
        /// The job name was not provided.
        /// or
        /// The job could not be found.
        /// </exception>
        public Job GetJob(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new FaultException("The job name was not provided.");

            this.Log("Job: {0}", jobName);

            try
            {
                var queryUri = new Uri(base.BaseUri, @"/job/" + jobName + "/");
                var job = base.Client.GetResource<Job>(queryUri, 1);
                return job;
            }
            catch (WebException)
            {
                throw new FaultException("The job could not be found.");
            }
        }

        /// <summary>
        ///     Gets all of the jobs that have been configured in jenkins.
        /// </summary>
        /// <returns>Returns a <see cref="IList{Job}" /> representing jobs configured.</returns>
        public IList<Job> GetJobs()
        {
            var tree = base.Client.GetResource<Tree>(base.BaseUri, 1);
            if (tree == null) return null;

            return tree.Jobs;
        }

        #endregion
    }
}