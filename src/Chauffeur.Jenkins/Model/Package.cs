using System;
using System.Runtime.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract(Name = "package", Namespace = "")]
    public class Package : IUrl
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Package" /> class.
        /// </summary>
        public Package()
        {
            this.Date = DateTime.Now.ToString("f");
            this.Machine = Environment.MachineName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        /// <value>
        ///     The build.
        /// </value>
        [DataMember(Name = "buildNumber")]
        public int BuildNumber { get; set; }

        /// <summary>
        ///     Gets or sets the change set.
        /// </summary>
        /// <value>
        ///     The change set.
        /// </value>
        [DataMember(Name = "changeSet")]
        public ChangeSet ChangeSet { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        /// <value>
        ///     The date.
        /// </value>
        [DataMember(Name = "date")]
        public string Date { get; set; }
       
        /// <summary>
        ///     Gets or sets the job.
        /// </summary>
        /// <value>
        ///     The job.
        /// </value>
        [DataMember(Name = "job")]
        public string Job { get; set; }

        /// <summary>
        ///     Gets or sets the machine.
        /// </summary>
        /// <value>
        ///     The machine.
        /// </value>
        [DataMember(Name = "machine")]
        public string Machine { get; set; }

        /// <summary>
        ///     Gets or sets the paths.
        /// </summary>
        /// <value>
        ///     The paths.
        /// </value>
        [DataMember(Name = "paths")]
        public string[] Paths { get; set; }

        #endregion

        #region IUrl Members

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        [DataMember(Name = "url", Order = 0)]
        public Uri Url { get; set; }

        #endregion
    }
}