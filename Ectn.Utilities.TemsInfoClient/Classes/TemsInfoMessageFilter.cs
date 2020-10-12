using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Ectn.Utilities.TemsInfo {

    public partial class TemsInfoMessageFilter {

        #region Props

        /// <summary>
        /// Describes which of the "Vehicle" messages should be sent.
        /// Default value: No message is sent
        /// </summary>
        [XmlElement]
        public AllowRoadVehicle[] VehicleMessages {
            get;
            set;
        }

        /// <summary>
        /// Describes which of the "Object Start" messages should be sent.
        /// Default value: No message is sent
        /// </summary>
        [XmlElement]
        public AllowRoadDevice[] ObjectStartMessages {
            get;
            set;
        }

        /// <summary>
        /// Describes which of the "Object Stop" messages should be sent.
        /// Default value: No message is sent
        /// </summary>
        [XmlElement]
        public AllowRoadDevice[] ObjectStopMessages {
            get;
            set;
        }

        /// <summary>
        /// Describes which of the "Object Location" messages should be sent.
        /// Default value: No message is sent
        /// </summary>
        [XmlElement]
        public AllowRoadDevice[] ObjectLocationMessages {
            get;
            set;
        }

        /// <summary>
        /// Describes which of the "Value" messages should be sent.
        /// Default value: No message is sent
        /// </summary>
        [XmlElement]
        public AllowRoadDeviceName[] ValueMessages {
            get;
            set;
        }

        /// <summary>
        /// Describes which of the "Data" messages should be sent.
        /// Default value: No message is sent
        /// </summary>
        [XmlElement]
        public AllowRoadDeviceName[] DataMessages {
            get;
            set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>TemsInfoMessageFilter</c> not allowing any messages to
        /// be sent.
        /// </summary>
        public TemsInfoMessageFilter() {
            VehicleMessages = new AllowRoadVehicle[0];
            ObjectStartMessages = new AllowRoadDevice[0];
            ObjectStopMessages = new AllowRoadDevice[0];
            ObjectLocationMessages = new AllowRoadDevice[0];
            ValueMessages = new AllowRoadDeviceName[0];
            DataMessages = new AllowRoadDeviceName[0];
        }

        #region Static Access

        /// <summary>
        /// Creates a <c>TemsInfoMessageFilter</c> allowing all messages to be
        /// sent except value and data messages.
        /// </summary>
        public static TemsInfoMessageFilter CreateAllowAllFilter() {
            return new TemsInfoMessageFilter() {
                VehicleMessages = new AllowRoadVehicle[] {
                    new AllowRoadVehicle() {
                        RoadNo = null,
                        BinaryDataTypes = new AllowVehicleBinary[] {
                            new AllowVehicleBinary() {
                                BinaryDataType = null,
                            },
                        },
                    },
                },
                ObjectStartMessages = new AllowRoadDevice[] {
                    new AllowRoadDevice() {
                        RoadNo = null,
                        DeviceNo = null,
                    },
                },
                ObjectStopMessages = new AllowRoadDevice[] {
                    new AllowRoadDevice() {
                        RoadNo = null,
                        DeviceNo = null,
                    },
                },
                ObjectLocationMessages = new AllowRoadDevice[] {
                    new AllowRoadDevice() {
                        RoadNo = null,
                        DeviceNo = null,
                    },
                },
                ValueMessages = new AllowRoadDeviceName[] { 
                    new AllowRoadDeviceName() {
                        RoadNo = null,
                        DeviceNo = null,
                        Name = AllowRoadDeviceName.ALLOW_ALL
                    }
                }
            };
        }

        #endregion Static Access

        #region Utils

        public class AllowRoad {

            #region Props

            /// <summary>
            /// Zero based number of the road (see Information message).
            /// If this attribute is missing, all roads are concerned.
            /// </summary>
            [XmlIgnore]
            public int? RoadNo {
                get;
                set;
            }

            [XmlAttribute("RoadNo")]
            [DefaultValue(int.MaxValue)]
            public int _RoadNo {
                get {
                    return RoadNo ?? int.MaxValue;
                }
                set {
                    RoadNo = value != int.MaxValue ? value : (int?)null;
                }
            }

            #endregion Props
        }

        public class AllowRoadDevice : AllowRoad {

            #region Props

            /// <summary>
            /// Zero based number of the device in the specified road (see Information message).
            /// If this attribute is missing, all devices on the road are concerned.
            /// </summary>
            [XmlIgnore]
            public int? DeviceNo {
                get;
                set;
            }

            [XmlAttribute("DeviceNo")]
            [DefaultValue(int.MaxValue)]
            public int _DeviceNo {
                get {
                    return DeviceNo ?? int.MaxValue;
                }
                set {
                    DeviceNo = value != int.MaxValue ? value : (int?)null;
                }
            }

            #endregion Props
        }

        public class AllowRoadDeviceName : AllowRoadDevice {

            public const string ALLOW_ALL = "*";

            #region Props

            /// <summary>
            /// Name of the value that should be sent.
            /// Use "*" if all values should be sent.
            /// </summary>
            [XmlAttribute]
            [DefaultValue(ALLOW_ALL)]
            public string Name {
                get;
                set;
            }

            #endregion Props

            public AllowRoadDeviceName() {
                Name = ALLOW_ALL;
            }
        }

        public class AllowRoadVehicle : AllowRoad {

            #region Props

            /// <summary>
            /// List of binary data types that should be sent.
            /// If this list is missing or contains no entries, no binary data
            /// is sent.
            /// 
            /// Default value: No binary data is sent
            /// </summary>
            [XmlElement]
            public AllowVehicleBinary[] BinaryDataTypes {
                get;
                set;
            }

            #endregion Props

            public AllowRoadVehicle() {
                BinaryDataTypes = new AllowVehicleBinary[0];
            }
        }

        public class AllowVehicleBinary {

            #region Props

            /// <summary>
            /// Type of the binary data that should be sent.
            /// Use <c>NULL</c> if all values should be sent.
            /// </summary>
            [XmlIgnore]
            public TemsInfoBinaryDataType? BinaryDataType {
                get;
                set;
            }

            [XmlAttribute("BinaryDataType")]
            [DefaultValue(TemsInfoBinaryDataType.Unknown)]
            public TemsInfoBinaryDataType _BinaryDataType {
                get {
                    return BinaryDataType ?? TemsInfoBinaryDataType.Unknown;
                }
                set {
                    BinaryDataType = value != TemsInfoBinaryDataType.Unknown ? value : (TemsInfoBinaryDataType?)null;
                }
            }

            #endregion Props
        }

        #endregion Utils
    }
}