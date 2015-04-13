using System;

namespace Chauffeur.Jenkins.Client
{
    public interface IUrl
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        Uri Url { get; set; }

        #endregion
    }
}