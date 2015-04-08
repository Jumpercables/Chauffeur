using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;

using Chauffeur.Jenkins.Client;
using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services;
using System.Collections.Generic;

using Chauffeur.Services;

namespace Chauffeur.Tests
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            string jobName = ConfigurationManager.AppSettings["jenkins.job"];
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Jenkins");
            
            ChauffeurService chauffeurService = new ChauffeurService();
            chauffeurService.InstallLastSuccessfulBuild(jobName, directory);
        }        

        #endregion
    }
}