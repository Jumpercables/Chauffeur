using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Configuration;
using Chauffeur.Jenkins.Model;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     Provides access to the changes that caused the build.
    /// </summary>
    [ServiceContract]
    public interface IChangeSetService
    {
        #region Public Methods

        /// <summary>
        ///     Gets the changes that caused the build.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>
        ///     Returns a <see cref="ChangeSet" /> representing the changes.
        /// </returns>
        Task<ChangeSet> GetChangesAsync(Build build);

        /// <summary>
        ///     Gets the changes that caused the build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns a <see cref="ChangeSet" /> representing the changes.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Changes/{jobName}/{buildNumber}", ResponseFormat = WebMessageFormat.Json)]
        Task<ChangeSet> GetChangesAsync(string jobName, string buildNumber);

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
    ///     Provides access to the changes that caused the build.
    /// </summary>
    public class ChangeSetService : JenkinsService, IChangeSetService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeSetService" /> class.
        /// </summary>
        public ChangeSetService()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeSetService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        internal ChangeSetService(Uri baseUri, JenkinsClient client, ChauffeurConfiguration configuration)
            : base(baseUri, client, configuration)
        {
        }

        #endregion

        #region IChangeSetService Members

        /// <summary>
        ///     Gets the changes that caused the build.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>
        ///     Returns a <see cref="ChangeSet" /> representing the changes.
        /// </returns>
        public async Task<ChangeSet> GetChangesAsync(Build build)
        {
            Log.Info(this, "Loading the change sets for the build: {0}", build);

            var causes = build.Actions.Select(o => o.Causes).FirstOrDefault(o => o.Any());
            if (causes != null)
            {
                var up = causes.FirstOrDefault(o => !string.IsNullOrEmpty(o.UpstreamBuild));
                if (up != null)
                {
                    var changeSet = await this.GetChangesAsync(up.UpstreamProject, up.UpstreamBuild);
                    return changeSet;
                }
            }

            return build.ChangeSet;
        }

        /// <summary>
        ///     Gets the changes that caused the build.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="buildNumber">The build number.</param>
        /// <returns>
        ///     Returns a <see cref="ChangeSet" /> representing the changes.
        /// </returns>
        public async Task<ChangeSet> GetChangesAsync(string jobName, string buildNumber)
        {
            var service = new JobService(base.BaseUri, base.Client, base.Configuration);
            var build = await service.GetBuildAsync(jobName, buildNumber);
            return await this.GetChangesAsync(jobName, build);
        }

        /// <summary>
        ///     Gets the URI templates that are availabe for the service.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="List{T}" /> representing the methods available.
        /// </returns>
        public List<string> GetUriTemplates()
        {
            return this.GetUriTemplates(typeof (ChangeSetService));
        }

        #endregion

        #region Private Methods

        private async Task<ChangeSet> GetChangesAsync(string jobName, Build build)
        {
            var service = new JobService(base.BaseUri, base.Client, base.Configuration);
            var lastFailedBuild = await service.GetLastFailedBuildAsync(jobName);
            if (lastFailedBuild.Number == build.Number)
            {
            }

            return null;
        }

        #endregion
    }
}