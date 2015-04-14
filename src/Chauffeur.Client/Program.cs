using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading.Tasks;

using Chauffeur.Client.ServiceReference;

namespace Chauffeur.Client
{
    internal class Program
    {
        #region Private Methods

        private static async Task<Build> InstallLastSuccessfulBuildAsync(string jobName, string machineName)
        {
            var remoteAddress = new EndpointAddress(string.Format("http://{0}:8080/Chauffeur.Jenkins.Services/ChauffeurService/", machineName));
            using (ChauffeurServiceClient client = new ChauffeurServiceClient("BasicHttpBinding_IChauffeurService", remoteAddress))
            {
                var build = await client.InstallLastSuccessfulBuildAsync(jobName);

                return build;
            }
        }
       
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid arguments: <jobName> <machineName>");
                return 1;
            }

            try
            {
                for (int i = 1; i < args.GetLength(0); i++)
                {
                    var build = Task.Run(() => InstallLastSuccessfulBuildAsync(args[0], args[i]));
                    Console.WriteLine("{0} - Last successful build requested to be installed: {1}", args[i], build.Result.number);                    
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return 2;
        }

        #endregion
    }
}