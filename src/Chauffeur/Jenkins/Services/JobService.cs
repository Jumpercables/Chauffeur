using System;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Model;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     A service used to obtain the job information from jenkins.
    /// </summary>
    public class JobService : JenkinsService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="JobService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        public JobService(Uri baseUri, JenkinsClient client)
            : base(baseUri, client)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the job from server with the specified name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>
        ///     Returns a <see cref="Job" /> representing the job.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">jobName</exception>
        public Job GetJob(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            var queryUri = new Uri(base.BaseUri, @"/job/" + jobName + "/");
            var job = base.Client.GetResource<Job>(queryUri, 1);
            return job;
        }        

        #endregion
    }
}