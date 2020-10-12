using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Vehicle" message contains all the information about a
    /// vehicle recorded by the TEMS Recorder.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgVehicle {

        /// <summary>
        /// Unique ID of the vehicle.
        /// 
        /// IDs are increasing but they do not necessarily start at 0 and
        /// certain values may be left out.
        /// 
        /// This ID corresponds to the one in
        /// <seealso cref="TemsInfoMsgObjectStart"/>,
        /// <seealso cref="TemsInfoMsgObjectStop"/> and
        /// <seealso cref="TemsInfoMsgObjectLocation"/> messages.
        /// </summary>
        public long objectId;

        /// <summary>
        /// Zero-based number of the road from the
        /// <seealso cref="TemsInfoMsgInformation"/> message.
        /// </summary>
        public int roadNo;

        /// <summary>
        /// Zero-based number of the lane on the road.
        /// </summary>
        public int laneNo;

        /// <summary>
        /// Zero-based number of the device that recorded this vehicle.
        /// </summary>
        public int deviceNo;

        /// <summary>
        /// Time when the front of the vehicle crossed the line Z = 0.
        /// </summary>
        public long timestampFront;

        /// <summary>
        /// Time when the rear end of the vehicle crossed the line Z = 0.
        /// </summary>
        public long timestampRear;

        /// <summary>
        /// Whether the vehicle is moving along the default direction
        /// ("Normal") or opposite to it ("Reverse").
        /// 
        /// The default direction is indicated in the
        /// <seealso cref="TemsInfoMsgInformation"/> message as part of the
        /// lane configuration.
        /// </summary>
        public TemsInfoVehicleDirection vehicleDirection;

        /// <summary>
        /// Width [m] of the vehicle.
        /// </summary>
        public float width;

        /// <summary>
        /// Height [m] of the vehicle.
        /// </summary>
        public float height;

        /// <summary>
        /// Length [m] of the vehicle.
        /// </summary>
        public float length;

        /// <summary>
        /// Speed [m/s] of the vehicle.
        /// 
        /// The speed is negative if the vehicle is moving opposite to the
        /// default direction.
        /// </summary>
        public float speed;

        /// <summary>
        /// Center [m] of the object relative to the device.
        /// </summary>
        public float deviceCenter;

        /// <summary>
        /// Center [m] of the object relative to the center of the lane.
        /// </summary>
        public float laneCenter;

        /// <summary>
        /// Center [m] of the object relative to the left border of the road.
        /// </summary>
        public float roadCenter;

        /// <summary>
        /// Number of binary datas.
        /// </summary>
        public int numberOfBinaryDatas;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgVehicle_BinaryData {

        /// <summary>
        /// Type of the binary data.
        /// </summary>
        public TemsInfoBinaryDataType type;

        /// <summary>
        /// Title / description of the binary data.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
        public string title;

        /// <summary>
        /// Payload of the binary data.
        /// </summary>
        public int numberOfBytes;
    }
}