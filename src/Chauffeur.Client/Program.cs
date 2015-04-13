using System;

using Chauffeur.Client.ServiceReference;

namespace Chauffeur.Client
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid arguments: <endPointConfigurationName> <jobName>");    
                    return;
                }

                ChauffeurServiceClient chauffeurServiceClient = new ChauffeurServiceClient(args[0]);
                var build = chauffeurServiceClient.InstallLastSuccessfulBuild(args[1]);
                Console.WriteLine("Last successful build installed: {0}", build.number);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}