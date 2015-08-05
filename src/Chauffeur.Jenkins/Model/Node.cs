using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract]
    public class Node : IUrl
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node" /> class.
        /// </summary>
        public Node()
        {
            this.Jobs = new List<Job>();
            this.Views = new List<View>();
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "jobs")]
        public IList<Job> Jobs { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }

        [DataMember(Name = "nodeDescription")]
        public string NodeDescription { get; set; }

        [DataMember(Name = "nodeName")]
        public string NodeName { get; set; }

        [DataMember(Name = "numExecutors")]
        public int NumExecutors { get; set; }

        [DataMember(Name = "primaryView")]
        public View PrimaryView { get; set; }

        [DataMember(Name = "quietingDown")]
        public bool QuietingDown { get; set; }

        [DataMember(Name = "slaveAgentPort")]
        public int SlaveAgentPort { get; set; }

        [DataMember(Name = "useCrumbs")]
        public bool UseCrumbs { get; set; }

        [DataMember(Name = "useSecurity")]
        public bool UseSecurity { get; set; }

        [DataMember(Name = "views")]
        public IList<View> Views { get; set; }

        #endregion

        #region IUrl Members

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        #endregion
    }
}