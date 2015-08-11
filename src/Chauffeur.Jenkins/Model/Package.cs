using System;
using System.Runtime.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract]
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

        [DataMember(Name = "build", Order = 4)]
        public Build Build { get; set; }

        [DataMember(Name = "date", Order = 2)]
        public string Date { get; set; }

        [DataMember(Name = "job", Order = 1)]
        public string Job { get; set; }

        [DataMember(Name = "paths", Order = 3)]
        public string[] Paths { get; set; }

        #endregion

        #region IUrl Members

        [DataMember(Name = "url", Order = 0)]
        public Uri Url { get; set; }

        #endregion
    }
}