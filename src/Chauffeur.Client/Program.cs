using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using Chauffeur.Client.ServiceReference;

namespace Chauffeur.Client
{
    internal class Program
    {
        #region Private Methods

        private async Task InstallLastSuccessfulBuildAsync(string jobName, string machineName)
        {
            var remoteAddress = new EndpointAddress(string.Format("http://{0}:8080/Chauffeur.Jenkins.Services/ChauffeurService/", machineName));
            using (ChauffeurServiceClient client = new ChauffeurServiceClient("BasicHttpBinding_IChauffeurService", remoteAddress))
            {
                try
                {
                    var build = await client.InstallLastSuccessfulBuildAsync(jobName);
                    Console.WriteLine("{0} - Last successful build requested to be installed: {1}", machineName, build.number);
                }
                catch (FaultException ex)
                {
                    Console.WriteLine(ex.Message);
                    client.Abort();
                }
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Invalid arguments: <jobName> <machineName>");
                }

                var tasks = new Program().Run(args);
                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


        private Task[] Run(string[] args)
        {
            List<string> machineNames = new List<string>();
            var tasks = new List<Task>();

            for (int i = 1; i < args.GetLength(0); i++)
            {
                if (!machineNames.Contains(args[i]))
                {
                    var machineName = args[i];
                    var task = Task.Run(() => InstallLastSuccessfulBuildAsync(args[0], machineName));
                    tasks.Add(task);
                }

                machineNames.Add(args[i]);
            }

            return tasks.ToArray();
        }

        #endregion
    }
}