#region License

// Copyright 2011 Jason Walker
// ungood@onetrue.name
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract]
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

        [DataMember(Name = "buildable")]
        public bool Buildable { get; set; }

        [DataMember(Name = "builds")]
        public IList<Build> Builds { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "concurrentBuild")]
        public bool ConcurrentBuild { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "downstreamProjects")]
        public IList<Job> DownstreamProjects { get; set; }

        [DataMember(Name = "firstBuild")]
        public Build FirstBuild { get; set; }

        [DataMember(Name = "healthReport")]
        public IList<HealthReport> HealthReports { get; set; }

        [DataMember(Name = "inQueue")]
        public bool InQueue { get; set; }

        [DataMember(Name = "keepDependencies")]
        public bool KeepDependencies { get; set; }

        [DataMember(Name = "lastBuild")]
        public Build LastBuild { get; set; }

        [DataMember(Name = "lastCompletedBuild")]
        public Build LastCompletedBuild { get; set; }

        [DataMember(Name = "lastFailedBuild")]
        public Build LastFailedBuild { get; set; }

        [DataMember(Name = "lastStableBuild")]
        public Build LastStableBuild { get; set; }

        [DataMember(Name = "lastSuccessfulBuild")]
        public Build LastSuccessfulBuild { get; set; }

        [DataMember(Name = "lastUnstableBuild")]
        public Build LastUnstableBuild { get; set; }

        [DataMember(Name = "lastUnsuccessfulBuild")]
        public Build LastUnsuccessfulBuild { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "nextBuildNumber")]
        public int NextBuildNumber { get; set; }

        [DataMember(Name = "upstreamProjects")]
        public IList<Job> UpstreamProjects { get; set; }

        #endregion

        #region IUrl Members

        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName ?? this.Name;
        }

        #endregion
    }

    [DataContract]
    public class HealthReport
    {
        #region Public Properties

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }

        #endregion
    }
}