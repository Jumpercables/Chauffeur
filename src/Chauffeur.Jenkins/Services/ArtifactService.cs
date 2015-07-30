using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Model;

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
        /// <param name="directory">The directory.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> that representing the paths to the local artifacts.
        /// </returns>
        [OperationContract]
        Task<string[]> DownloadArtifactsAsync(Build build, string directory);

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
        /// <exception cref="System.ArgumentNullException">
        ///     baseUri
        ///     or
        ///     client
        /// </exception>
        internal ArtifactService(Uri baseUri, JenkinsClient client)
            : base(baseUri, client)
        {
        }

        #endregion

        #region IArtifactService Members

        /// <summary>
        ///     Downloads all of the artifacts from the build into the specified directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{String}" /> that representing the paths to the local artifacts.
        /// </returns>
        public Task<string[]> DownloadArtifactsAsync(Build build, string directory)
        {
            if (build == null)
                throw new WebFaultException<ErrorData>(new ErrorData("The build was not provided.", "The build argument must be specified."), HttpStatusCode.BadRequest);

            if (directory == null)
                throw new WebFaultException<ErrorData>(new ErrorData("The directory was not provided.", "The directory argument must be specified."), HttpStatusCode.BadRequest);           

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            this.Log("Downloading {0} artifact(s) into the {1} directory.", build.Artifacts.Count, directory);

            return Task.Run(() =>
            {
                var tasks = build.Artifacts.Select(artifact => this.DownloadArtifactAsync(build, artifact, directory));
                return tasks.Select(o => o.Result).ToArray();
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
            WebRequest request = this.CreateRequest(build, artifact);

            using (var response = request.GetResponse())
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
            }

            return fileName;
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="artifact">The artifact.</param>
        /// <returns>
        /// Returns a <see cref="WebRequest" /> representing the request to the server.
        /// </returns>
        private WebRequest CreateRequest(Build build, Artifact artifact)
        {
            try
            {
                Uri absoluteUri = new Uri(build.Url, @"artifact/" + artifact.RelativePath);
                return base.Client.GetRequest(absoluteUri);
            }
            catch (WebException)
            {
                throw new WebFaultException<ErrorData>(new ErrorData("The artifact was not found.", "The build artifact: {0} " + artifact.RelativePath + " was not found on the server."), HttpStatusCode.NotFound);
            }
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
                this.Log("\t{0}: {1} = {2}.", artifact.FileName, task.Result, task.Status);

                return task.Result;
            });
        }

        #endregion
    }
}