using System;
using System.Net;
using System.Threading;

namespace Chauffeur.Jenkins.System
{
    /// <summary>
    ///     A simple HTTP request listener that redirects the requests that the caller can perform an action when a request is
    ///     received.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class ChaufferServiceListener : IDisposable
    {
        #region Fields

        private readonly HttpListener _Listener;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChaufferServiceListener" /> class.
        /// </summary>
        /// <param name="prefixes">The prefixes.</param>
        /// <exception cref="NotSupportedException">Needs Windows XP SP2, Server 2003 or later.</exception>
        /// <exception cref="ArgumentException">prefixes</exception>
        public ChaufferServiceListener(params string[] prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            _Listener = new HttpListener();

            foreach (var prefix in prefixes)
                _Listener.Prefixes.Add(prefix);

            _Listener.Start();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is listening.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is listening; otherwise, <c>false</c>.
        /// </value>
        public bool IsListening
        {
            get { return _Listener.IsListening; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Starts listening to the HTTP requests and calls the specified action when the request is intercepted.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Run(Action<HttpListenerRequest> action)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    while (_Listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            var context = c as HttpListenerContext;
                            if (context == null) return;

                            try
                            {
                                if (action != null)
                                    action(context.Request);
                            }
                            catch
                            {
                                // Suppress any exceptions
                            }
                            finally
                            {
                                // Always close the stream
                                context.Response.OutputStream.Close();
                            }
                        }, _Listener.GetContext());
                    }
                }
                catch
                {
                    // Suppress any exceptions
                }
            });
        }

        /// <summary>
        ///     Stops listening for HTTP requests.
        /// </summary>
        public void Stop()
        {
            if (_Listener.IsListening)
                _Listener.Stop();

            _Listener.Close();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }

        #endregion
    }
}