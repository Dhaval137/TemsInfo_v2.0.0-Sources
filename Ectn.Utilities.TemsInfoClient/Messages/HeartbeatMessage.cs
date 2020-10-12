using System;
using System.Text;

namespace Ectn.Utilities.TemsInfo {

    public partial class HeartbeatMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Heartbeat;
            }
        }

        public uint SequenceNo {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>HearbeatMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public HeartbeatMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            TemsInfoMsgHeartbeat msg = ByteArrayHelper.GetMessage<TemsInfoMsgHeartbeat>(content);
            SequenceNo = msg.sequenceNo;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TEMS Info Message of type \"{0}\":{1}", MessageType, Environment.NewLine);
            sb.AppendFormat("   Sequence Number = {0:#,##0}", SequenceNo);
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}