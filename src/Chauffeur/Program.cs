using System.ServiceProcess;

using Chauffeur.Windows.Services;

namespace Chauffeur
{
    internal static class Program
    {
        #region Private Methods

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ChauffeurService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        #endregion
    }
}