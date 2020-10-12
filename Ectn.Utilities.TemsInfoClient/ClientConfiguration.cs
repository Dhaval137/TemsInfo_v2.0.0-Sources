using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Ectn.Utilities.TemsInfo {

    [XmlRoot(
        Namespace = SCHEMA_TARGET_NAMESPACE, 
        ElementName = ClientConfiguration.ROOT_ELEMENT_NAME)]
    public partial class ClientConfiguration {

        #region Constant Values

        internal const string ROOT_ELEMENT_NAME = "TemsInfoConfiguration";

        private const string SCHEMA_TARGET_NAMESPACE = "http://code.sch.sick.com/TemsInfoConfiguration.xsd";
        private const string XML_INDENT_CHARS = "    ";

        #endregion Constant Values

        #region Props

        /// <summary>
        /// Specifies the credentials which define the rights of the connected
        /// client.
        /// 
        /// Default value: <c>NULL</c> -> <seealso cref="TemsInfoUserLevel.Operator"/>
        /// </summary>
        [XmlElement("Credentials")]
        public TemsInfoCredentials Credentials { get; set; }

        /// <summary>
        /// Specifies the time [s] between two "HeartbeatMessage"s.
        /// "0" means that no "HeartbeatMessage"s are sent.
        /// 
        /// Default value: 0
        /// </summary>
        [XmlElement]
        public float HeartbeatMessageInterval { get; set; }

        /// <summary>
        /// How "Status" messages should be triggered.
        /// Default value: periodically.
        /// </summary>
        [XmlElement]
        public TemsInfoStatusMessageMode StatusMessageMode { get; set; } = TemsInfoStatusMessageMode.Periodically;

        /// <summary>
        /// If the periodical sending of "Status" messages is enabled
        /// (<seealso cref="StatusMessageMode"/>) the next message is sent no
        /// later than this many seconds.
        /// 
        /// Default value: 20
        /// </summary>
        [XmlElement]
        public float StatusMessageInterval { get; set; } = 20;

        /// <summary>
        /// Specifies which of the available messages should be sent to the
        /// client.
        /// </summary>
        [XmlElement]
        public TemsInfoMessageFilter MessageFilter { get; set; }

        /// <summary>
        /// Enables the recovery mechanism for the "Client Name" specified in
        /// the "Configuration" message. When the connection is lost and
        /// re-established (with the same "Client Name") the TEMS Info server
        /// sends all messages that couldn't be sent while the connection was
        /// lost.
        /// </summary>
        [XmlElement]
        public TemsInfoRecoveryBehavior RecoveryBehavior { get; set; }

        #endregion Props

        #region Static Access

        #region Validation

        /// <summary>
        /// Checks whether given file is a valid client configuration file.
        /// </summary>
        /// <returns><c>True</c> if the file is a valid client configuration file, otherwise <c>False</c>.</returns>
        public static bool Validate(string filename) {
            return Validate(XDocument.Load(filename));
        }

        /// <summary>
        /// Checks whether given file is a valid client configuration.
        /// </summary>
        /// <returns><c>True</c> if the document is a valid client configuration, otherwise <c>False</c>.</returns>
        public static bool Validate(XDocument document) {
            string[] messages;
            return Validate(document, out messages);
        }

        /// <summary>
        /// Checks whether given file is a valid client configuration file.
        /// </summary>
        /// <param name="messages">Describes what exactly does not match for a valid client configuration.</param>
        /// <returns><c>True</c> if the file is a valid client configuration file, otherwise <c>False</c>.</returns>
        public static bool Validate(string filename, out string[] messages) {
            return Validate(
                XDocument.Load(filename),
                out messages);
        }

        /// <summary>
        /// Checks whether given document is a valid client configuration.
        /// </summary>
        /// <param name="messages">Describes what exactly does not match for a valid client configuration.</param>
        /// <returns><c>True</c> if the document is a valid client configuration, otherwise <c>False</c>.</returns>
        public static bool Validate(XDocument document, out string[] messages) {
            List<string> validationMessages = new List<string>();
            XmlSchemaSet schemaSet = GetSchema();
            document.Validate(schemaSet, (validationSender, validationArgs) => {
                validationMessages.Add(string.Format("{0}: {1}", validationArgs.Severity, validationArgs.Message));
            });

            messages = validationMessages.ToArray();
            return validationMessages.Count == 0;
        }

        #endregion Validation

        /// <summary>
        /// Loads a <c>ClientConfiguration</c> from given file.
        /// </summary>
        public static ClientConfiguration Load(string filename) {
            using (FileStream stream = File.OpenRead(filename)) {
                return Deserialize(stream);
            }
        }

        /// <summary>
        /// Loads a <c>ClientConfiguration</c> from given stream.
        /// </summary>
        public static ClientConfiguration Deserialize(Stream stream) {
            return new XmlSerializer(typeof(ClientConfiguration)).Deserialize(stream) as ClientConfiguration;
        }

        /// <summary>
        /// Loads a <c>ClientConfiguration</c> from given XML string.
        /// </summary>
        public static ClientConfiguration FromSerializedXml(string xml) {
            return FromBytes(Encoding.UTF8.GetBytes(xml));
        }

        /// <summary>
        /// Loads a <c>ClientConfiguration</c> from given bytes.
        /// </summary>
        public static ClientConfiguration FromBytes(byte[] bytes) {
            using (MemoryStream stream = new MemoryStream(bytes)) {
                return Deserialize(stream);
            }
        }

        #endregion Static Access

        #region Public Access

        /// <summary>
        /// Serializes this <c>ClientConfiguration</c> using UTF8 to a XML
        /// string.
        /// </summary>
        public string ToSerializedXml() {
            return Encoding.UTF8.GetString(ToBytes());
        }

        /// <summary>
        /// Serializes this <c>ClientConfiguration</c> using UTF8 and converts
        /// it to bytes.
        /// </summary>
        public byte[] ToBytes() {
            using (MemoryStream stream = new MemoryStream()) {
                Serialize(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Serializes this <c>ClientConfiguration</c> into given stream using
        /// UTF8 encoding.
        /// </summary>
        public void Serialize(Stream stream) {
            XmlWriterSettings settings = new XmlWriterSettings() {
                Encoding = new UTF8Encoding(false),
                Indent = XML_INDENT_CHARS.Length > 0,
                IndentChars = XML_INDENT_CHARS,
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings)) {
                new XmlSerializer(typeof(ClientConfiguration)).Serialize(xmlWriter, this);
            }
        }

        /// <summary>
        /// Creates or overwrites given file and serializes this
        /// <c>ClientConfiguration</c> using UTF8 and stores it
        /// into the file.
        /// </summary>
        public void Save(string filename) {
            using (FileStream stream = File.Create(filename)) {
                Serialize(stream);
            }
        }

        #endregion Public Access

        #region Utils

        private static XmlSchemaSet GetSchema() {
            XmlSchemaSet result = new XmlSchemaSet();
            XDocument schema = XDocument.Parse(Properties.Resources.TemsInfoConfiguration);
            result.Add(SCHEMA_TARGET_NAMESPACE, schema.CreateReader());
            return result;
        }

        #endregion Utils
    }
}