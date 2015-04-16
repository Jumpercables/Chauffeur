using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
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
        ///     Returns a <see cref="IEnumerable{String}" /> that representing the paths to the local artifacts.
        /// </returns>
        [OperationContract]
        List<string> DownloadArtifacts(Build build, string directory);

        #endregion
    }

    /// <summary>
    /// A service used to obtain the build artifacts from jenkins.
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

        #region Public Methods

        /// <summary>
        /// Downloads all of the artifacts from the build into the specified directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>
        /// Returns a <see cref="IEnumerable{String}" /> that representing the paths to the local artifacts.
        /// </returns>
        /// <exception cref="System.ServiceModel.FaultException">
        /// The build was not provided.
        /// or
        /// The directory was not provided.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">build
        /// or
        /// directory</exception>
        public List<string> DownloadArtifacts(Build build, string directory)
        {
            if (build == null)
                throw new FaultException("The build was not provided.");

            if (directory == null)
                throw new FaultException("The directory was not provided.");

            this.Log("Downloading build {0} to '{1}'.", build.Number, directory);

            var tasks = build.Artifacts.Select(artifact => this.DownloadArtifactAsync(build, artifact, directory)).ToArray();
            Task.WaitAll(tasks);

            return tasks.Select(o => o.Result).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Downloads the artifact into the directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="artifact">The artifact.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>
        /// Returns a <see cref="string" /> representing the full path to the local artifact.
        /// </returns>
        /// <exception cref="System.ServiceModel.FaultException">
        /// The build was not provided.
        /// or
        /// The artifact was not provided.
        /// or
        /// The directory was not provided.
        /// </exception>
        private string DownloadArtifact(Build build, Artifact artifact, string directory)
        {
            if (build == null)
                throw new FaultException("The build was not provided.");

            if(artifact == null)
                throw new FaultException("The artifact was not provided.");

            if (directory == null)
                throw new FaultException("The directory was not provided.");

            string fileName = Path.Combine(directory, artifact.FileName);
            Uri absoluteUri = new Uri(build.Url, @"artifact/" + artifact.RelativePath);
            var request = base.Client.GetRequest(absoluteUri);

            using (var response = request.GetResponse())
            {
                // Once the WebResponse object has been retrieved,
                // get the stream object associated with the response's data
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        // Create the local file
                        using (var localStream = File.Create(fileName))
                        {
                            // Allocate a 1k buffer
                            byte[] buffer = new byte[1024];
                            int bytes;

                            // Simple do/while loop to read from stream until
                            // no bytes are returned
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
        ///     Asynchronously downloads the artifact into the directory.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="artifact">The artifact.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>Returns a <see cref="string" /> representing the full path to the local artifact.</returns>
        private Task<string> DownloadArtifactAsync(Build build, Artifact artifact, string directory)
        {
            return Task.Run(() => this.DownloadArtifact(build, artifact, directory));
        }

        #endregion
    }
}