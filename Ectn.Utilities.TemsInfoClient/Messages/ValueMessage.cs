using System;
using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    public partial class ValueMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Value;
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
        /// Name of the value.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Data type of the value.
        /// </summary>
        public TemsInfoDataType Type {
            get;
            private set;
        }

        public object OldValue {
            get;
            private set;
        }

        public object NewValue {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>ValueMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public ValueMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            int offset = 0;
            TemsInfoMsgValue msgValue = ByteArrayHelper.GetMessagePart<TemsInfoMsgValue>(content, ref offset);
            this.Timestamp = new DateTime(msgValue.timestamp);
            this.RoadNo = msgValue.roadNo;
            this.DeviceNo = msgValue.deviceNo;
            this.Name = msgValue.name;
            this.Type = msgValue.type;

            OldValue = ReadValue(this.Type, content, ref offset);
            NewValue = ReadValue(this.Type, content, ref offset);
        }

        #endregion Overridden

        #region Utils

        private object ReadValue(TemsInfoDataType type, byte[] content, ref int offset) {
            int size = BitConverter.ToInt32(content, offset);
            offset += 4;
            switch (type) {
                case TemsInfoDataType.Bool_1:
                    byte? result = ReadByte(content, ref offset, size);
                    return result.HasValue ? (bool?)(result.Value != 0) : (bool?)null;
                case TemsInfoDataType.UInt_8:
                    return ReadByte(content, ref offset, size);
                case TemsInfoDataType.Int_16:
                    return size > 0 ? (short?)ByteArrayHelper.GetMessagePart<short>(content, ref offset) : null;
                case TemsInfoDataType.UInt_16:
                    return size > 0 ? (ushort?)ByteArrayHelper.GetMessagePart<ushort>(content, ref offset) : null;
                case TemsInfoDataType.Int_32:
                    return size > 0 ? (int?)ByteArrayHelper.GetMessagePart<int>(content, ref offset) : null;
                case TemsInfoDataType.UInt_32:
                    return size > 0 ? (uint?)ByteArrayHelper.GetMessagePart<uint>(content, ref offset) : null;
                case TemsInfoDataType.Int_64:
                    return size > 0 ? (long?)ByteArrayHelper.GetMessagePart<long>(content, ref offset) : null;
                case TemsInfoDataType.DateTime:
                    return size > 0 ? (DateTime?)new DateTime(ByteArrayHelper.GetMessagePart<long>(content, ref offset)) : null;
                case TemsInfoDataType.Float_32:
                    return size > 0 ? (float?)ByteArrayHelper.GetMessagePart<float>(content, ref offset) : null;
                case TemsInfoDataType.Float_64:
                    return size > 0 ? (double?)ByteArrayHelper.GetMessagePart<double>(content, ref offset) : null;
                case TemsInfoDataType.String:
                case TemsInfoDataType.Unknown:
                default:
                    if (size > 0) {
                        string value = System.Text.Encoding.UTF8.GetString(content, offset, size);
                        offset += size;
                        return value;
                    } else {
                        return null;
                    }
            }
        }

        private byte? ReadByte(byte[] content, ref int offset, int size) {
            return size > 0 ? (byte?)ByteArrayHelper.GetMessagePart<byte>(content, ref offset) : null;
        }

        #endregion Utils
    }
}