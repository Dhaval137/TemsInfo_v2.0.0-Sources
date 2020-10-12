using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ectn.Utilities.TemsInfo {

    public partial class DataMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Data;
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
        /// Zero-based number of the road according to the "Information"
        /// message.
        /// </summary>
        public int RoadNo {
            get;
            private set;
        }

        /// <summary>
        /// Number of the device that triggered this event.
        /// </summary>
        public int DeviceNo {
            get;
            private set;
        }

        /// <summary>
        /// Name of the data source.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Name of the data type used for the data payload.
        /// </summary>
        public string Type {
            get;
            private set;
        }

        /// <summary>
        /// The data as a sequence of bytes.
        /// </summary>
        public byte[] Data {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>DataMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public DataMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            int offset = 0;
            TemsInfoMsgData msgData = ByteArrayHelper.GetMessagePart<TemsInfoMsgData>(content, ref offset);
            this.Timestamp = new DateTime(msgData.timestamp);
            this.RoadNo = msgData.roadNo;
            this.DeviceNo = msgData.deviceNo;
            this.Name = msgData.name;
            this.Type = msgData.type;
            this.Data = new byte[msgData.dataSize];
            Array.Copy(content, offset, this.Data, 0, this.Data.Length);
        }

        #endregion Overridden
    }
}