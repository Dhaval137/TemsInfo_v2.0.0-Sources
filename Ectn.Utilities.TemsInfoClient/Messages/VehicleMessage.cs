using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfo {

    public partial class VehicleMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Vehicle;
            }
        }

        public long ObjectId {
            get;
            private set;
        }

        public int RoadNo {
            get;
            private set;
        }

        public int LaneNo {
            get;
            private set;
        }

        public int DeviceNo {
            get;
            private set;
        }

        public DateTime TimestampFront {
            get;
            private set;
        }

        public DateTime TimestampRear {
            get;
            private set;
        }

        public TemsInfoVehicleDirection VehicleDirection {
            get;
            private set;
        }

        public float HeightMeter {
            get;
            private set;
        }

        public float WidthMeter {
            get;
            private set;
        }

        public float LengthMeter {
            get;
            private set;
        }

        public float SpeedMeterPerSecond {
            get;
            private set;
        }

        public float DeviceCenterMeter {
            get;
            private set;
        }

        public float LaneCenterMeter {
            get;
            private set;
        }

        public float RoadCenterMeter {
            get;
            private set;
        }

        public List<BinaryData> Data {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>VehicleMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public VehicleMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            int offset = 0;

            TemsInfoMsgVehicle msg = ByteArrayHelper.GetMessagePart<TemsInfoMsgVehicle>(content, ref offset);
            ObjectId = msg.objectId;
            RoadNo = msg.roadNo;
            LaneNo = msg.laneNo;
            DeviceNo = msg.deviceNo;
            TimestampFront = new DateTime(msg.timestampFront);
            TimestampRear = new DateTime(msg.timestampRear);
            VehicleDirection = msg.vehicleDirection;
            WidthMeter = msg.width;
            HeightMeter = msg.height;
            LengthMeter = msg.length;
            SpeedMeterPerSecond = msg.speed;
            DeviceCenterMeter = msg.deviceCenter;
            LaneCenterMeter = msg.laneCenter;
            RoadCenterMeter = msg.roadCenter;

            if (msg.numberOfBinaryDatas > 0) {
                Data = new List<BinaryData>(msg.numberOfBinaryDatas);
                for (int idxBinaryData = 0; idxBinaryData < msg.numberOfBinaryDatas; idxBinaryData++) {
                    Data.Add(GetBinaryData(content, ref offset));
                }
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TEMS Info Message of type \"{0}\":{1}", MessageType, Environment.NewLine);
            sb.AppendFormat("   Object ID = {0:#,##0}", ObjectId);
            sb.AppendLine();
            sb.AppendFormat("   Road Number = {0}", RoadNo);
            sb.AppendLine();
            sb.AppendFormat("   Lane Number = {0}", LaneNo);
            sb.AppendLine();
            sb.AppendFormat("   Device Number = {0}", DeviceNo);
            sb.AppendLine();
            sb.AppendFormat("   Timestamp Front = {0:dd.MM.yyyy HH:mm:ss.fff}", TimestampFront);
            sb.AppendLine();
            sb.AppendFormat("   Timestamp Rear  = {0:dd.MM.yyyy HH:mm:ss.fff}", TimestampRear);
            sb.AppendLine();
            sb.AppendFormat("   Vehicle Direction = {0}", VehicleDirection);
            sb.AppendLine();
            sb.AppendFormat("   Width = {0:#,##0.000}m", WidthMeter);
            sb.AppendLine();
            sb.AppendFormat("   Height = {0:#,##0.000}m", HeightMeter);
            sb.AppendLine();
            sb.AppendFormat("   Length = {0:#,##0.000}m", LengthMeter);
            sb.AppendLine();
            sb.AppendFormat("   Speed = {0:#,##0.000}m/s", SpeedMeterPerSecond);
            sb.AppendLine();
            sb.AppendFormat("   Device Center = {0:#,##0.000}m", DeviceCenterMeter);
            sb.AppendLine();
            sb.AppendFormat("   Lane Center = {0:#,##0.000}m", LaneCenterMeter);
            sb.AppendLine();
            sb.AppendFormat("   Road Center = {0:#,##0.000}m", RoadCenterMeter);
            sb.AppendLine();
            sb.AppendFormat("   Binary Data = {0:#,##0} ({1:#,##0}Bytes, {2} Vehicle XML, {3} Zipped Vehicle Data, {4} Point Cloud(s), {5} JPEGs)",
                Data != null ? Data.Count : 0,
                Data != null ? Data.Sum(data => data.Data.Length) : 0,
                Data != null ? Data.Count(data => data is VehicleXmlData) : 0,
                Data != null ? Data.Count(data => data is ZippedVehicleData) : 0,
                Data != null ? Data.Count(data => data is PointCloudData) : 0,
                Data != null ? Data.Count(data => data is ImageData) : 0);
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden

        #region Utils

        private BinaryData GetBinaryData(byte[] content, ref int offset) {
            TemsInfoMsgVehicle_BinaryData msg = ByteArrayHelper.GetMessagePart<TemsInfoMsgVehicle_BinaryData>(content, ref offset);
            byte[] buffer = new byte[msg.numberOfBytes];
            Array.Copy(content, offset, buffer, 0, buffer.Length);
            offset += buffer.Length;
            switch (msg.type) {
                case TemsInfoBinaryDataType.VehicleXml:
                    return new VehicleXmlData(msg.title, buffer);
                case TemsInfoBinaryDataType.Vehicle:
                    return new ZippedVehicleData(msg.title, buffer);
                case TemsInfoBinaryDataType.PointCloud:
                    return new PointCloudData(msg.title, buffer);
                case TemsInfoBinaryDataType.TimedPointCloud:
                    return new TimedPointCloudData(msg.title, buffer);
                case TemsInfoBinaryDataType.Jpeg:
                    return new ImageData(msg.title, buffer);
                default:
                    throw new NotImplementedException();
            }
        }

        public partial class BinaryData {

            #region Props

            /// <summary>
            /// Title / description of the binary data.
            /// </summary>
            public string Title {
                get;
                private set;
            }

            /// <summary>
            /// Payload of the binary data.
            /// </summary>
            public byte[] Data {
                get;
                protected set;
            }

            #endregion Props

            public BinaryData(string title, byte[] data) {
                this.Title = title;
                this.Data = data;
            }
        }

        public partial class ZippedVehicleData : BinaryData {

            public ZippedVehicleData(string title, byte[] data)
                : base(title, data) {
            }
        }

        public partial class VehicleXmlData : BinaryData {

            #region Props

            public XDocument VehicleXml {
                get;
                private set;
            }

            #endregion Props

            public VehicleXmlData(string title, byte[] data)
                : base(title, data) {

                using (MemoryStream stream = new MemoryStream(data)) {
                    VehicleXml = XDocument.Load(stream);
                }
            }
        }

        public partial class PointCloudData : BinaryData {

            #region Props

            public TemsInfoBinary3D Binary3D {
                get;
                private set;
            }

            #endregion Props

            public PointCloudData(string title, byte[] data)
                : base(title, data) {

                this.Binary3D = new TemsInfoBinary3D(data);
            }
        }

        public partial class TimedPointCloudData : BinaryData {

            #region Props

            public TemsInfoTimedBinary3D TimedBinary3D {
                get;
                private set;
            }

            #endregion Props

            public TimedPointCloudData(string title, byte[] data)
                : base(title, data) {

                this.TimedBinary3D = new TemsInfoTimedBinary3D(data);
            }
        }

        public partial class ImageData : BinaryData {

            #region Props

            public Image Image {
                get;
                private set;
            }

            #endregion Props

            public ImageData(string title, byte[] data)
                : base(title, data) {

                using (MemoryStream stream = new MemoryStream(data)) {
                    this.Image = Image.FromStream(stream);
                }
            }
        }

        #endregion Utils
    }
}