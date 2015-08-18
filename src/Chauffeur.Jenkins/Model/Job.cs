using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract(Name = "job")]
    public class Job : IUrl
    {
        #region Constructors

        public Job()
        {
            this.Builds = new List<Build>();
            this.DownstreamProjects = new List<Job>();
            this.UpstreamProjects = new List<Job>();
            this.HealthReports = new List<HealthReport>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Job" /> is buildable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if buildable; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "buildable")]
        public bool Buildable { get; set; }

        /// <summary>
        ///     Gets or sets the builds.
        /// </summary>
        /// <value>
        ///     The builds.
        /// </value>
        [DataMember(Name = "builds")]
        public IList<Build> Builds { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        [DataMember(Name = "color")]
        public string Color { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [concurrent build].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [concurrent build]; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "concurrentBuild")]
        public bool ConcurrentBuild { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the downstream projects.
        /// </summary>
        /// <value>
        ///     The downstream projects.
        /// </value>
        [DataMember(Name = "downstreamProjects")]
        public IList<Job> DownstreamProjects { get; set; }

        /// <summary>
        ///     Gets or sets the first build.
        /// </summary>
        /// <value>
        ///     The first build.
        /// </value>
        [DataMember(Name = "firstBuild")]
        public Build FirstBuild { get; set; }

        /// <summary>
        ///     Gets or sets the health reports.
        /// </summary>
        /// <value>
        ///     The health reports.
        /// </value>
        [DataMember(Name = "healthReport")]
        public IList<HealthReport> HealthReports { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [in queue].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [in queue]; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "inQueue")]
        public bool InQueue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [keep dependencies].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [keep dependencies]; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "keepDependencies")]
        public bool KeepDependencies { get; set; }

        /// <summary>
        ///     Gets or sets the last build.
        /// </summary>
        /// <value>
        ///     The last build.
        /// </value>
        [DataMember(Name = "lastBuild")]
        public Build LastBuild { get; set; }

        /// <summary>
        ///     Gets or sets the last completed build.
        /// </summary>
        /// <value>
        ///     The last completed build.
        /// </value>
        [DataMember(Name = "lastCompletedBuild")]
        public Build LastCompletedBuild { get; set; }

        /// <summary>
        ///     Gets or sets the last failed build.
        /// </summary>
        /// <value>
        ///     The last failed build.
        /// </value>
        [DataMember(Name = "lastFailedBuild")]
        public Build LastFailedBuild { get; set; }

        /// <summary>
        ///     Gets or sets the last stable build.
        /// </summary>
        /// <value>
        ///     The last stable build.
        /// </value>
        [DataMember(Name = "lastStableBuild")]
        public Build LastStableBuild { get; set; }

        /// <summary>
        ///     Gets or sets the last successful build.
        /// </summary>
        /// <value>
        ///     The last successful build.
        /// </value>
        [DataMember(Name = "lastSuccessfulBuild")]
        public Build LastSuccessfulBuild { get; set; }

        /// <summary>
        ///     Gets or sets the last unstable build.
        /// </summary>
        /// <value>
        ///     The last unstable build.
        /// </value>
        [DataMember(Name = "lastUnstableBuild")]
        public Build LastUnstableBuild { get; set; }

        /// <summary>
        ///     Gets or sets the last unsuccessful build.
        /// </summary>
        /// <value>
        ///     The last unsuccessful build.
        /// </value>
        [DataMember(Name = "lastUnsuccessfulBuild")]
        public Build LastUnsuccessfulBuild { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the next build number.
        /// </summary>
        /// <value>
        ///     The next build number.
        /// </value>
        [DataMember(Name = "nextBuildNumber")]
        public int NextBuildNumber { get; set; }

        /// <summary>
        ///     Gets or sets the upstream projects.
        /// </summary>
        /// <value>
        ///     The upstream projects.
        /// </value>
        [DataMember(Name = "upstreamProjects")]
        public IList<Job> UpstreamProjects { get; set; }

        #endregion

        #region IUrl Members

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName ?? this.Name;
        }

        #endregion
    }

    [DataContract(Name = "healthReport")]
    public class HealthReport
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the icon URL.
        /// </summary>
        /// <value>
        ///     The icon URL.
        /// </value>
        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        [DataMember(Name = "score")]
        public int Score { get; set; }

        #endregion
    }
}