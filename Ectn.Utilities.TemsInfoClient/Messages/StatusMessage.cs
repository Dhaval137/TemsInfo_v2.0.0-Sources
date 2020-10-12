using System;
using System.Text;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfo {

    public partial class StatusMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Status;
            }
        }

        /// <summary>
        /// Time when the message was sent.
        /// </summary>
        public DateTime Timestamp {
            get;
            private set;
        }

        /// <summary>
        /// Global health state of the TEMS system.
        /// </summary>
        public TemsInfoHealthState SystemState {
            get;
            private set;
        }

        /// <summary>
        /// Detailed description of the system status.
        /// </summary>
        public XDocument Status {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>StatusMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public StatusMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            int offset = 0;

            TemsInfoMsgStatus msgStatus = ByteArrayHelper.GetMessagePart<TemsInfoMsgStatus>(content, ref offset);
            Timestamp = new DateTime(msgStatus.timestamp);
            SystemState = msgStatus.systemState;
            Status = ByteArrayHelper.GetXDocument(content, ref offset, msgStatus.statusSize);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TEMS Info Message of type \"{0}\":{1}", MessageType, Environment.NewLine);
            sb.AppendFormat("   Timestamp = {0:dd.MM.yyyy HH:mm:ss.fff}", Timestamp);
            sb.AppendLine();
            sb.AppendFormat("   System State = {0}", SystemState);
            sb.AppendLine();
            sb.AppendLine("Detailed Status:");
            sb.AppendLine(Status.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}