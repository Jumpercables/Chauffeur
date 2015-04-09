using System;
using System.Configuration;
using System.IO;

using Chauffeur.Windows.Services;

namespace Chauffeur.Tests
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            string jobName = ConfigurationManager.AppSettings["job"];
            string artifactsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jenkins");

            ChauffeurService chauffeurService = new ChauffeurService();
            chauffeurService.InstallLastSuccessfulBuild(jobName, artifactsDirectory);
        }

        #endregion
    }
}