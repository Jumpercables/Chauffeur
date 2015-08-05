using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Chauffeur.Jenkins.Configuration
{
    public class ChauffeurConfiguration
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChauffeurConfiguration" /> class.
        /// </summary>
        public ChauffeurConfiguration()
        {
            this.LoadConfigurationAndInitialize(ConfigurationManager.AppSettings.AllKeys.Select(k => Tuple.Create(k, ConfigurationManager.AppSettings[k])));
        }

        #endregion

        #region Public Properties

        public JenkinsTypedSettings Jenkins { get; private set; }
        public NotificationsTypedSettings Notifications { get; private set; }
        public PackagesTypedSettings Packages { get; private set; }

        #endregion

        #region Private Properties

        private NameValueCollection Settings { get; set; }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            this.Jenkins = new JenkinsTypedSettings(this.Settings);
            this.Packages = new PackagesTypedSettings(this.Settings);
            this.Notifications = new NotificationsTypedSettings(this.Settings);
        }

        private void LoadConfigurationAndInitialize(IEnumerable<Tuple<string, string>> values)
        {
            this.Settings = new NameValueCollection();
            
            foreach (Tuple<string, string> tuple in values)
            {
                if (tuple.Item1.StartsWith("Chauffeur/", StringComparison.OrdinalIgnoreCase))
                    this.Settings[tuple.Item1] = tuple.Item2;
            }

            this.Initialize();
        }

        #endregion

        #region Nested Type: BooleanSetting

        internal class BooleanSetting : TypedSetting<bool>
        {
            #region Constructors

            public BooleanSetting(NameValueCollection settings, string name, bool value)
                : base(settings, name, value)
            {
            }

            #endregion
        }

        #endregion

        #region Nested Type: StringSetting

        internal class StringSetting : TypedSetting<string>
        {
            #region Constructors

            public StringSetting(NameValueCollection settings, string name, string value)
                : base(settings, name, value)
            {
            }

            #endregion
        }

        #endregion

        #region Nested Type: StronglyTypedSetting

        internal abstract class TypedSetting<TValue>
        {
            #region Constructors

            public TypedSetting(NameValueCollection settings, string name, TValue value)
            {
                this.Value = string.IsNullOrEmpty(settings[name]) ? value : (TValue) Convert.ChangeType(settings[name], typeof (TValue));
            }

            #endregion

            #region Public Properties

            public TValue Value { get; private set; }

            #endregion
        }

        #endregion
    }
}