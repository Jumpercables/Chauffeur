using System;

using Chauffeur.Client.ServiceReference;

namespace Chauffeur.Client
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            ChauffeurServiceClient chauffeurServiceClient = new ChauffeurServiceClient();
            foreach (var jobName in args)
            {
                var build = chauffeurServiceClient.InstallLastSuccessfulBuildAsync(jobName);
                Console.WriteLine("Build: {0}", build);
            }
        }

        #endregion
    }
}