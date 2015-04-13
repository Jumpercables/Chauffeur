using Chauffeur.Jenkins.Services;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Chauffeur.WindowsService
{
    /// <summary>
    ///     A windows service used to monitor the builds and automatically install the last succsseful build.
    /// </summary>
    public partial class ChauffeurWindowService : ServiceBase
    {
        #region Fields

        private ServiceHost _ServiceHost;

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
            if (_ServiceHost != null)
            {
                _ServiceHost.Close();
            }

            // Create a ServiceHost for the ChauffeurService type and 
            // provide the base address.
            _ServiceHost = new ServiceHost(typeof(ChauffeurService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            _ServiceHost.Open();
        }

        /// <summary>
        ///     When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control
        ///     Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            if (_ServiceHost != null)
            {
                _ServiceHost.Close();
                _ServiceHost = null;
            }
        }

        #endregion
    }
}