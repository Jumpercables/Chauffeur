using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract]
    public class Build : IUrl
    {
        #region Constructors

        public Build()
        {
            this.Culprits = new List<User>();
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "artifacts")]
        public IList<Artifact> Artifacts { get; set; }

        [DataMember(Name = "building")]
        public bool Building { get; set; }

        [DataMember(Name = "buildOn")]
        public string BuiltOn { get; set; }

        [DataMember(Name = "changeSet")]
        public ChangeSet ChangeSet { get; set; }

        [DataMember(Name = "culprits")]
        public IList<User> Culprits { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "fullDisplayName")]
        public string FullDisplayName { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "keepLog")]
        public bool KeepLog { get; set; }

        [DataMember(Name = "number")]
        public int Number { get; set; }

        [DataMember(Name = "result")]
        public string Result { get; set; }        

        #endregion

        #region IUrl Members

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
            return this.FullDisplayName ?? this.Number.ToString();
        }

        #endregion
    }

    [DataContract]
    public class Artifact
    {
        #region Public Properties

        [DataMember(Name = "displayPath")]
        public string DisplayPath { get; set; }

        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

        [DataMember(Name = "relativePath")]
        public string RelativePath { get; set; }

        #endregion
    }

    [DataContract]
    public class ChangeSet
    {
        #region Constructors

        public ChangeSet()
        {
            this.Items = new List<ChangeSetItem>();
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "items")]
        public IList<ChangeSetItem> Items { get; set; }

        #endregion
    }

    [DataContract]
    public class ChangeSetItem
    {
        #region Constructors

        public ChangeSetItem()
        {
            this.Paths = new List<ChangeSetPath>();
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "author")]
        public User Author { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "msg")]
        public string Message { get; set; }

        [DataMember(Name = "paths")]
        public IList<ChangeSetPath> Paths { get; set; }

        #endregion
    }

    [DataContract]
    public class ChangeSetPath
    {
        #region Public Properties

        [DataMember(Name = "editType")]
        public string EditType { get; set; }

        [DataMember(Name = "file")]
        public string File { get; set; }

        #endregion
    }
}