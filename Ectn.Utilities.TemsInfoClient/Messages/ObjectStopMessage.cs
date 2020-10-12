using System;
using System.Text;

namespace Ectn.Utilities.TemsInfo {

    public partial class ObjectStopMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.ObjectStop;
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

        public TemsInfoTriggerResult Result {
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

        public long[] TrackedFrontTriggerIds {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>ObjectStopMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public ObjectStopMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            int offset = 0;

            TemsInfoMsgObjectStop msg = ByteArrayHelper.GetMessagePart<TemsInfoMsgObjectStop>(content, ref offset);
            Timestamp = new DateTime(msg.timestamp);
            TimestampEvent = new DateTime(msg.timestampEvent);
            ObjectId = msg.objectId;
            Result = msg.result;
            RoadNo = msg.roadNo;
            DeviceNo = msg.deviceNo;
            StartXMeter = msg.startX;
            StopXMeter = msg.stopX;

            TrackedFrontTriggerIds = new long[msg.numberOfTrackedFrontTriggerIds];
            for (int idxTrigger = 0; idxTrigger < TrackedFrontTriggerIds.Length; idxTrigger++) {
                TemsInfoMsgObjectStop_TrackedFrontTriggerId msgTrigger = ByteArrayHelper.GetMessagePart<TemsInfoMsgObjectStop_TrackedFrontTriggerId>(content, ref offset);
                TrackedFrontTriggerIds[idxTrigger] = msgTrigger.id;
            }
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
            sb.AppendFormat("   Result = {0}", Result);
            sb.AppendLine();
            sb.AppendFormat("   Road = {0}", RoadNo);
            sb.AppendLine();
            sb.AppendFormat("   Device = {0}", DeviceNo);
            sb.AppendLine();
            sb.AppendFormat("   Start X = {0:#,##0.000}", StartXMeter);
            sb.AppendLine();
            sb.AppendFormat("   Stop X = {0:#,##0.000}", StopXMeter);
            sb.AppendLine();
            if (TrackedFrontTriggerIds.Length > 0) {
                sb.AppendFormat("   Tracked Front Trigger IDs = ", string.Join(", ", TrackedFrontTriggerIds));
            }
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}