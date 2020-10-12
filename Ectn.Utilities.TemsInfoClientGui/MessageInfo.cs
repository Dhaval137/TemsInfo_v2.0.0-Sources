using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

using Ectn.Utilities.TemsInfo;

namespace Ectn.Utilities.TemsInfoClientGui {

    public class MessageInfo : PropertyChangedBase {

        #region Props

        public DateTime Timestamp {
            get;
            private set;
        }

        public TemsInfoMessageType MessageType {
            get;
            private set;
        }

        public ObservableCollection<MessageInfoValue> Values {
            get;
            private set;
        }

        public ObservableCollection<DocumentInfo> Documents {
            get;
            private set;
        }

        #endregion Props

        public MessageInfo(InformationMessage message)
            : this(message.MessageType) {

            AddValue("Server Version", GetVersionString(message.ServerVersion));

            AddDocument("Recorder Configuration", message.RecorderConfiguration);
            AddDocument("Site Configuration", message.SiteConfiguration);
        }

        public MessageInfo(ErrorMessage message)
            : this(message.MessageType) {

            AddValue("Error Code", "{0}", message.Code);
            AddValue("Message", message.Message);
        }

        public MessageInfo(VehicleMessage message)
            : this(message.MessageType) {

            AddValue("Front Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.TimestampFront);
            AddValue("Rear Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.TimestampRear);
            AddValue("Object ID", "{0:#,##0}", message.ObjectId);
            AddValue("Road #", "{0:#,##0}", message.RoadNo);
            AddValue("Lane #", "{0:#,##0}", message.LaneNo);
            AddValue("Device #", "{0:#,##0}", message.DeviceNo);
            AddValue("Direction", "{0}", message.VehicleDirection);
            AddValue("Height", "{0:#,##0.000}m", message.HeightMeter);
            AddValue("Width", "{0:#,##0.000}m", message.WidthMeter);
            AddValue("Length", "{0:#,##0.000}m", message.LengthMeter);
            AddValue("Speed", "{0:#,##0.000}m/s", message.SpeedMeterPerSecond);
            AddValue("Device Center", "{0:#,##0.000}m", message.DeviceCenterMeter);
            AddValue("Lane Center", "{0:#,##0.000}m", message.LaneCenterMeter);
            AddValue("Road Center", "{0:#,##0.000}m", message.RoadCenterMeter);
            AddValue("Data", "{0:#,##0} ({1:#,##0} Bytes)",
                message.Data != null ? message.Data.Count : 0,
                message.Data != null ? message.Data.Sum(data => data.Data.Length) : 0);

            if (message.Data != null) {
                VehicleMessage.VehicleXmlData data = message.Data.FirstOrDefault(d => d is VehicleMessage.VehicleXmlData) as VehicleMessage.VehicleXmlData;
                if (data != null) {
                    AddDocument(data.Title, data.VehicleXml);
                }
            }
        }

        public MessageInfo(ObjectStartMessage message)
            : this(message.MessageType) {

            AddValue("Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.Timestamp);
            AddValue("Event Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.TimestampEvent);
            AddValue("Object ID", "{0:#,##0}", message.ObjectId);
            AddValue("Road #", "{0:#,##0}", message.RoadNo);
            AddValue("Device #", "{0:#,##0}", message.DeviceNo);
            AddValue("Start X", "{0:#,##0.000}m", message.StartXMeter);
            AddValue("Stop X", "{0:#,##0.000}m", message.StopXMeter);
        }

        public MessageInfo(ObjectStopMessage message)
            : this(message.MessageType) {

            AddValue("Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.Timestamp);
            AddValue("Event Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.TimestampEvent);
            AddValue("Object ID", "{0:#,##0}", message.ObjectId);
            AddValue("Road #", "{0:#,##0}", message.RoadNo);
            AddValue("Device #", "{0:#,##0}", message.DeviceNo);
            AddValue("Result", "{0}", message.Result);
            AddValue("Start X", "{0:#,##0.000}m", message.StartXMeter);
            AddValue("Stop X", "{0:#,##0.000}m", message.StopXMeter);
            AddValue("Trigger IDs", string.Join(", ", message.TrackedFrontTriggerIds));
        }

        public MessageInfo(ObjectLocationMessage message)
            : this(message.MessageType) {

            AddValue("Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.Timestamp);
            AddValue("Event Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.TimestampEvent);
            AddValue("Object ID", "{0:#,##0}", message.ObjectId);
            AddValue("Road #", "{0:#,##0}", message.RoadNo);
            AddValue("Device #", "{0:#,##0}", message.DeviceNo);
            AddValue("Trigger Position", "{0:#,##0.000}m", message.TriggerLocationMeter);
            AddValue("Vehicle Position", "{0:#,##0.000}m", message.LocationMeter);
            AddValue("Moving Direction", "{0}", message.MovingDirection);
            AddValue("Accuracy", "{0}", message.Accuracy);
        }

        public MessageInfo(StatusMessage message)
            : this(message.MessageType) {

            AddValue("Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.Timestamp);
            AddValue("System State", "{0}", message.SystemState);

            AddDocument("Recorder Status", message.Status);
        }

        public MessageInfo(ValueMessage message)
            : this(message.MessageType) {

            AddValue("Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.Timestamp);
            AddValue("Name", "{0}", message.Name);
            AddValue("Road #", "{0:#,##0}", message.RoadNo);
            AddValue("Device #", "{0:#,##0}", message.DeviceNo);
            AddValue("Old Value", "{0}", message.OldValue);
            AddValue("New Value", "{0}", message.NewValue);
        }

        public MessageInfo(DataMessage message)
            : this(message.MessageType) {

            AddValue("Timestamp", "{0:d} {0:HH:mm:ss.fff}", message.Timestamp);
            AddValue("Name", "{0}", message.Name);
            AddValue("Road #", "{0:#,##0}", message.RoadNo);
            AddValue("Device #", "{0:#,##0}", message.DeviceNo);
            AddValue("Data Size", "{0:#,##0} Bytes", message.Data.Length);
        }

        private MessageInfo(TemsInfoMessageType type) {
            Timestamp = DateTime.Now;
            MessageType = type;
        }

        #region Public Access

        public void DeleteTemporaryDocumentFiles() {
            if (Documents != null) {
                foreach (DocumentInfo doc in Documents) {
                    doc.DeleteTemporaryFile();
                }
            }
        }

        #endregion Public Access

        #region Utils

        private string GetVersionString(TemsInfoVersion version) {
            switch (version) {
                case TemsInfoVersion.Version10:
                    return "v1.0";
                case TemsInfoVersion.Version11:
                    return "v1.1";
                case TemsInfoVersion.Version12:
                    return "v1.2";
                case TemsInfoVersion.Version20:
                    return "v2.0";
                default:
                    return "Unknown";
            }
        }

        private void AddValue(string name, string format, params object[] value) {
            AddValue(name, string.Format(format, value));
        }

        private void AddValue(string name, string value) {
            if (this.Values == null) {
                this.Values = new ObservableCollection<MessageInfoValue>();
            }
            this.Values.Add(new MessageInfoValue(name, value));
        }

        private void AddDocument(string title, XDocument document) {
            if (this.Documents == null) {
                this.Documents = new ObservableCollection<DocumentInfo>();
            }
            this.Documents.Add(new DocumentInfo(title, document));
        }

        #endregion Utils
    }

    public class MessageInfoValue : PropertyChangedBase {

        #region Props

        public string Name {
            get;
            private set;
        }

        public string Value {
            get;
            private set;
        }

        #endregion Props

        public MessageInfoValue(string name, string value) {
            this.Name = name;
            this.Value = value;
        }
    }
}