using Chauffeur.Console.ServiceReference;

namespace Chauffeur.Console
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            ChauffeurServiceClient chauffeurServiceClient = new ChauffeurServiceClient();
            foreach (var jobName in args)
                chauffeurServiceClient.InstallLastSuccessfulBuildAsync(jobName);
        }

        #endregion
    }
}