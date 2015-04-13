using System.ServiceProcess;

namespace Chauffeur.WindowsService
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
                new ChauffeurServiceHost(),
            };
            ServiceBase.Run(ServicesToRun);
        }

        #endregion
    }
}