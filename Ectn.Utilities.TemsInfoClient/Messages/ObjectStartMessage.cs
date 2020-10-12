using System;
using System.Text;

namespace Ectn.Utilities.TemsInfo {

    public partial class ObjectStartMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.ObjectStart;
            }
        }

        public DateTime Timestamp {
            get;
            private set;
        }

        public DateTime TimestampEvent {
            get;
            private set;
        }

        public long ObjectId {
            get;
            private set;
        }

        public int RoadNo {
            get;
            private set;
        }

        public int DeviceNo {
            get;
            private set;
        }

        public float StartXMeter {
            get;
            private set;
        }

        public float StopXMeter {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>ObjectStartMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public ObjectStartMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            TemsInfoMsgObjectStart msg = ByteArrayHelper.GetMessage<TemsInfoMsgObjectStart>(content);
            Timestamp = new DateTime(msg.timestamp);
            TimestampEvent = new DateTime(msg.timestampEvent);
            ObjectId = msg.objectId;
            RoadNo = msg.roadNo;
            DeviceNo = msg.deviceNo;
            StartXMeter = msg.startX;
            StopXMeter = msg.stopX;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TEMS Info Message of type \"{0}\":{1}", MessageType, Environment.NewLine);
            sb.AppendFormat("   Timestamp        = {0:dd.MM.yyyy HH:mm:ss.fff}", Timestamp);
            sb.AppendLine();
            sb.AppendFormat("   Timestamp Event  = {0:dd.MM.yyyy HH:mm:ss.fff} [{1:#,##0} ms]", TimestampEvent, (TimestampEvent - Timestamp).TotalMilliseconds);
            sb.AppendLine();
            sb.AppendFormat("   Object ID = {0:#,##0}", ObjectId);
            sb.AppendLine();
            sb.AppendFormat("   Road = {0}", RoadNo);
            sb.AppendLine();
            sb.AppendFormat("   Device = {0}", DeviceNo);
            sb.AppendLine();
            sb.AppendFormat("   Start X = {0:#,##0.000}", StartXMeter);
            sb.AppendLine();
            sb.AppendFormat("   Stop X = {0:#,##0.000}", StopXMeter);
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}