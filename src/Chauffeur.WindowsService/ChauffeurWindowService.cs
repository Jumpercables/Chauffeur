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
    public partial class ChauffeurWindowService : ServiceBase
    {
        #region Fields

        private ServiceHost _ServiceHost;
        private CancellationTokenSource _CancellationTokenSource;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurWindowService" /> class.
        /// </summary>
        public ChauffeurWindowService()
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
            _CancellationTokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                if (_ServiceHost != null)
                {
                    _ServiceHost.Close();
                    _ServiceHost = null;
                }
                                                
                _ServiceHost = new ServiceHost(typeof (ChauffeurService));
                _ServiceHost.Open();

            }, _CancellationTokenSource.Token);
        }

        /// <summary>
        ///     When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control
        ///     Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _CancellationTokenSource.Cancel();

            if (_ServiceHost != null)
            {
                if(_ServiceHost.State != CommunicationState.Closed || _ServiceHost.State != CommunicationState.Closing)
                    _ServiceHost.Close();

                _ServiceHost = null;
            }
        }

        #endregion
    }
}