using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Chauffeur.Jenkins.Model;
using Chauffeur.Jenkins.Services;

namespace Chauffeur.WindowsService
{
    /// <summary>
    ///     A windows service used to monitor the builds and automatically install the last succsseful build.
    /// </summary>
    public partial class ChauffeurServiceHost : ServiceBase
    {
        #region Fields

        private ServiceHost _ServiceHost;
        private CancellationToken _CancellationToken;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurServiceHost" /> class.
        /// </summary>
        public ChauffeurServiceHost()
        {
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     When implemented in a derived class, executes when a Start command is sent to the service by the Service Control
        ///     Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to
        ///     take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                _CancellationToken = new CancellationToken(false);
                _CancellationToken.Register(() => _ServiceHost.Close());
                
                _ServiceHost = new ServiceHost(typeof (ChauffeurService));
            }, _CancellationToken);
        }

        /// <summary>
        ///     When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control
        ///     Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _CancellationToken.ThrowIfCancellationRequested();
            _ServiceHost.Close();
        }

        #endregion
    }
}