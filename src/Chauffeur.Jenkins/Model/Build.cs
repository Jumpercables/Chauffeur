using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

using Chauffeur.Jenkins.Client;

namespace Chauffeur.Jenkins.Model
{
    [DataContract]
    public class Build : IUrl
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Build" /> class.
        /// </summary>
        public Build()
        {
            this.Culprits = new List<User>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the artifacts.
        /// </summary>
        /// <value>
        ///     The artifacts.
        /// </value>
        [DataMember(Name = "artifacts")]
        public IList<Artifact> Artifacts { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Build" /> is building.
        /// </summary>
        /// <value>
        ///     <c>true</c> if building; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "building")]
        public bool Building { get; set; }

        /// <summary>
        ///     Gets or sets the built on.
        /// </summary>
        /// <value>
        ///     The built on.
        /// </value>
        [DataMember(Name = "builtOn")]
        public string BuiltOn { get; set; }

        /// <summary>
        ///     Gets or sets the change set.
        /// </summary>
        /// <value>
        ///     The change set.
        /// </value>
        [DataMember(Name = "changeSet")]
        public ChangeSet ChangeSet { get; set; }

        /// <summary>
        ///     Gets or sets the culprits.
        /// </summary>
        /// <value>
        ///     The culprits.
        /// </value>
        [DataMember(Name = "culprits")]
        public IList<User> Culprits { get; set; }

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
        ///     Gets or sets the duration.
        /// </summary>
        /// <value>
        ///     The duration.
        /// </value>
        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        /// <summary>
        ///     Gets or sets the full name of the display.
        /// </summary>
        /// <value>
        ///     The full name of the display.
        /// </value>
        [DataMember(Name = "fullDisplayName")]
        public string FullDisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [keep log].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [keep log]; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "keepLog")]
        public bool KeepLog { get; set; }

        /// <summary>
        ///     Gets or sets the number.
        /// </summary>
        /// <value>
        ///     The number.
        /// </value>
        [DataMember(Name = "number")]
        public int Number { get; set; }

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        /// <value>
        ///     The result.
        /// </value>
        [DataMember(Name = "result")]
        public string Result { get; set; }

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

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.FullDisplayName ?? this.Number.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }

    [DataContract]
    public class Artifact
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the display path.
        /// </summary>
        /// <value>
        ///     The display path.
        /// </value>
        [DataMember(Name = "displayPath")]
        public string DisplayPath { get; set; }

        /// <summary>
        ///     Gets or sets the name of the file.
        /// </summary>
        /// <value>
        ///     The name of the file.
        /// </value>
        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the relative path.
        /// </summary>
        /// <value>
        ///     The relative path.
        /// </value>
        [DataMember(Name = "relativePath")]
        public string RelativePath { get; set; }

        #endregion
    }

    [DataContract]
    public class ChangeSet
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeSet" /> class.
        /// </summary>
        public ChangeSet()
        {
            this.Items = new List<ChangeSetItem>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        [DataMember(Name = "items")]
        public IList<ChangeSetItem> Items { get; set; }

        #endregion
    }

    [DataContract]
    public class ChangeSetItem
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeSetItem" /> class.
        /// </summary>
        public ChangeSetItem()
        {
            this.Paths = new List<ChangeSetPath>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the affected paths.
        /// </summary>
        /// <value>
        ///     The affected paths.
        /// </value>
        [DataMember(Name = "affectedPaths")]
        public IList<string> AffectedPaths { get; set; }

        /// <summary>
        ///     Gets or sets the author.
        /// </summary>
        /// <value>
        ///     The author.
        /// </value>
        [DataMember(Name = "author")]
        public User Author { get; set; }

        /// <summary>
        ///     Gets or sets the comment.
        /// </summary>
        /// <value>
        ///     The comment.
        /// </value>
        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        /// <value>
        ///     The date.
        /// </value>
        [DataMember(Name = "date")]
        public string Date { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember(Name = "version")]
        public string Version { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        [DataMember(Name = "msg")]
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the paths.
        /// </summary>
        /// <value>
        ///     The paths.
        /// </value>
        [DataMember(Name = "items")]
        public IList<ChangeSetPath> Paths { get; set; }

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        [DataMember(Name = "user")]
        public string User { get; set; }

        #endregion
    }

    [DataContract]
    public class ChangeSetPath
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the type of the edit.
        /// </summary>
        /// <value>
        ///     The type of the edit.
        /// </value>
        [DataMember(Name = "editType")]
        public string EditType { get; set; }

        /// <summary>
        ///     Gets or sets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        #endregion
    }
}