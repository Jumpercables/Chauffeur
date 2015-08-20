using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Chauffeur.Jenkins.Templates
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StyleSheetTemplate<T>
    {
        #region Fields

        private readonly string _FileName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StyleSheetTemplate{T}" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public StyleSheetTemplate(string fileName)
        {
            _FileName = fileName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Applies the template of the contents with the package to return an HTML output.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the HTML contents.
        /// </returns>
        public string ApplyTemplate(T data)
        {
            var contents = this.Serialize(data);
            var doc = XDocument.Load(new StringReader(contents));

            using (Stream stream = new FileStream(_FileName, FileMode.Open))
                return this.Transform(doc, stream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Serializes the specified data into XML.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns a <see cref="string" /> representing the data in XML format.</returns>
        protected virtual string Serialize(T data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof (T));
                using (XmlWriter xw = XmlWriter.Create(sw, settings))
                    serializer.WriteObject(xw, data);

                return sw.ToString();
            }
        }

        /// <summary>
        ///     Transforms the XML document with the contents of the stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the transformed output.
        /// </returns>
        protected string Transform(XDocument source, Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (StringWriter output = new StringWriter(new StringBuilder(), CultureInfo.InvariantCulture))
            {
                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    xslt.Load(XmlReader.Create(stream));
                    xslt.Transform(source.CreateReader(), writer);
                }

                return output.ToString();
            }
        }

        #endregion
    }
}