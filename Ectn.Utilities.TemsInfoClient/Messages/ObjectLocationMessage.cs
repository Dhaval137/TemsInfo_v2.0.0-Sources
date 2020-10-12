using System;
using System.Text;

namespace Ectn.Utilities.TemsInfo {

    public partial class ObjectLocationMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.ObjectLocation;
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

        public float TriggerLocationMeter {
            get;
            private set;
        }

        public float LocationMeter {
            get;
            private set;
        }

        public TemsInfoMovingDirection MovingDirection {
            get;
            private set;
        }

        public TemsInfoLocationAccuracy Accuracy {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>ObjectLocationMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public ObjectLocationMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            TemsInfoMsgObjectLocation msg = ByteArrayHelper.GetMessage<TemsInfoMsgObjectLocation>(content);
            Timestamp = new DateTime(msg.timestamp);
            TimestampEvent = new DateTime(msg.timestampEvent);
            ObjectId = msg.objectId;
            RoadNo = msg.roadNo;
            DeviceNo = msg.deviceNo;
            TriggerLocationMeter = msg.triggerLocation;
            LocationMeter = msg.location;
            MovingDirection = msg.movingDirection;
            Accuracy = msg.accuracy;
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
            sb.AppendFormat("   Trigger Location = {0:#,##0.000}m", TriggerLocationMeter);
            sb.AppendLine();
            sb.AppendFormat("   Location = {0:#,##0.000}m", LocationMeter);
            sb.AppendLine();
            sb.AppendFormat("   Moving Direction = {0}", MovingDirection);
            sb.AppendLine();
            sb.AppendFormat("   Accuracy = {0}", Accuracy);
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}