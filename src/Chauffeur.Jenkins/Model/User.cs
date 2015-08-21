using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract(Name = "user", Namespace = "")]
    public class User : IUrl
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the full name.
        /// </summary>
        /// <value>
        ///     The full name.
        /// </value>
        [DataMember(Name = "fullName")]
        public string FullName { get; set; }

        #endregion

        #region IUrl Members

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        [DataMember(Name = "absoluteUrl")]
        [XmlIgnore]
        public Uri Url { get; set; }

        #endregion
    }
}