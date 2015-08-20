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
            this.Date = DateTime.Now.ToShortDateString();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        /// <value>
        ///     The build.
        /// </value>
        [DataMember(Name = "build", Order = 4)]
        public Build Build { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        /// <value>
        ///     The date.
        /// </value>
        [DataMember(Name = "date", Order = 2)]
        public string Date { get; set; }

        /// <summary>
        ///     Gets or sets the job.
        /// </summary>
        /// <value>
        ///     The job.
        /// </value>
        [DataMember(Name = "job", Order = 1)]
        public string Job { get; set; }

        /// <summary>
        ///     Gets or sets the name of the machine.
        /// </summary>
        /// <value>
        ///     The name of the machine.
        /// </value>
        [DataMember(Name = "machineName")]
        public string MachineName { get; set; }

        /// <summary>
        ///     Gets or sets the paths.
        /// </summary>
        /// <value>
        ///     The paths.
        /// </value>
        [DataMember(Name = "paths", Order = 3)]
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