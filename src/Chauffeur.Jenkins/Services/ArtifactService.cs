﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Configuration;
using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.System;

namespace Chauffeur.Jenkins.Services
{
    /// <summary>
    ///     Provides a WCF service contract for artifacts within the Jenkins server.
    /// </summary>
    [ServiceContract]
    public interface IArtifactService
    {
        #region Public Methods

        /// <summary>
        ///     Downloads all of the artifacts from the build into the specified directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> that representing the paths to the local artifacts.
        /// </returns>
        Task<string[]> DownloadArtifactsAsync(Build build);

        #endregion
    }

    /// <summary>
    ///     A service used to obtain the build artifacts from jenkins.
    /// </summary>
    public class ArtifactService : JenkinsService, IArtifactService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArtifactService" /> class.
        /// </summary>
        public ArtifactService()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArtifactService" /> class.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="client">The client.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        internal ArtifactService(Uri baseUri, JenkinsClient client, ChauffeurConfiguration configuration)
            : base(baseUri, client, configuration)
        {
        }

        #endregion

        #region IArtifactService Members

        /// <summary>
        ///     Downloads all of the artifacts from the build into the specified directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{String}" /> that representing the paths to the local artifacts.
        /// </returns>
        public Task<string[]> DownloadArtifactsAsync(Build build)
        {
            Log.Info(this, "Downloading {0} artifact(s) into the {1} directory.", build.Artifacts.Count, this.Configuration.Packages.ArtifactsDirectory);

            return Task.Run(() =>
            {
                NetworkDrive drive = null;

                if (this.Configuration.Packages.IsMapDriveRequired)
                {
                    drive = NetworkDrive.Map(this.Configuration.Packages.ArtifactsDirectory, this.Configuration.Packages.Credentials);                    
                }

                using (drive)
                {
                    var tasks = build.Artifacts.Select(artifact => this.DownloadArtifactAsync(build, artifact, this.Configuration.Packages.ArtifactsDirectory));
                    return tasks.Select(o => o.Result).ToArray();
                }
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Downloads the artifact into the directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="artifact">The artifact.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the full path to the local artifact.
        /// </returns>
        private string DownloadArtifact(Build build, Artifact artifact, string directory)
        {
            string fileName = Path.Combine(directory, artifact.FileName);
            Uri absoluteUri = new Uri(build.Url, @"artifact/" + artifact.RelativePath);

            return base.Client.GetResource(absoluteUri, response =>
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        // Create the local file
                        using (var localStream = File.Create(fileName))
                        {
                            // Allocate the buffer
                            byte[] buffer = new byte[1024];
                            int bytes;

                            do
                            {
                                // Read data (up to 1k) from the stream
                                bytes = stream.Read(buffer, 0, buffer.Length);

                                // Write the data to the local file
                                localStream.Write(buffer, 0, bytes);
                            } while (bytes > 0);
                        }
                    }
                }

                return fileName;
            });
        }

        /// <summary>
        ///     Asynchronously downloads the artifact into the directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="artifact">The artifact.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>Returns a <see cref="string" /> representing the full path to the local artifact.</returns>
        private Task<string> DownloadArtifactAsync(Build build, Artifact artifact, string directory)
        {
            return Task.Run(() => this.DownloadArtifact(build, artifact, directory)).ContinueWith((task) =>
            {
                Log.Info(this, "\t{0} = {1}.", artifact.FileName, task.Status);

                return task.Result;
            });
        }

        #endregion
    }
}