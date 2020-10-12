using System;
using System.Text;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfo {

    public partial class InformationMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Information;
            }
        }

        public TemsInfoVersion ServerVersion {
            get;
            private set;
        }

        public XDocument RecorderConfiguration {
            get;
            private set;
        }

        public XDocument SiteConfiguration {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>InformationMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public InformationMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            int offset = 0;

            TemsInfoMsgInformation_Part1 msgPart1 = ByteArrayHelper.GetMessagePart<TemsInfoMsgInformation_Part1>(content, ref offset);
            ServerVersion = msgPart1.serverVersion;
            RecorderConfiguration = ByteArrayHelper.GetXDocument(content, ref offset, msgPart1.recorderConfigurationSize);

            TemsInfoMsgInformation_Part2 msgPart2 = ByteArrayHelper.GetMessagePart<TemsInfoMsgInformation_Part2>(content, ref offset);
            if (msgPart2.siteConfigurationSize > 0) {
                SiteConfiguration = ByteArrayHelper.GetXDocument(content, ref offset, msgPart2.siteConfigurationSize);
            } else {
                SiteConfiguration = null;
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TEMS Info Message of type \"{0}\":{1}", MessageType, Environment.NewLine);
            sb.AppendFormat("   Server Version = {0}.{1}",
                (uint)ServerVersion >> 16,
                (uint)ServerVersion & 0xFFFF);
            sb.AppendLine();
            sb.AppendLine("Recorder Configuration:");
            sb.AppendLine(RecorderConfiguration.ToString());
            sb.AppendLine();
            sb.AppendLine("Site Configuration:");
            if (SiteConfiguration != null) {
                sb.AppendLine(SiteConfiguration.ToString());
            }
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}