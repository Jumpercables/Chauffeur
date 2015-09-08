using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Chauffeur.Jenkins.Services.Templates
{
    public static class StyleSheetTemplate
    {
        #region Public Methods

        /// <summary>
        ///     Applies the template of the contents with the package to return an HTML output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the HTML contents.
        /// </returns>
        public static string ApplyTemplate<T>(string fileName, T data)
        {
            var contents = Serialize<T>(data);
            var doc = XDocument.Load(new StringReader(contents));

            using (Stream stream = new FileStream(fileName, FileMode.Open))
                return Transform(doc, stream);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Serializes the specified data into XML.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns a <see cref="string" /> representing the data in XML format.</returns>
        private static string Serialize<T>(T data)
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
        private static string Transform(XDocument source, Stream stream)
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