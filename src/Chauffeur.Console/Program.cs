using System;
using System.Configuration;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Services;

namespace Chauffeur.Console
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            string url = ConfigurationManager.AppSettings["server"];
            Uri baseUri = new Uri(url);

            string user = ConfigurationManager.AppSettings["user"];
            string token = ConfigurationManager.AppSettings["token"];

            JsonJenkinsClient client = new JsonJenkinsClient(user, token);
            ChauffeurService service = new ChauffeurService(baseUri, client);

            foreach (var jobName in args)
            {
                service.InstallLastSuccessfulBuild(jobName);
            }
        }

        #endregion
    }
}