using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Data" message is sent by the TEMS system every time data is
    /// produced by one of the configured sources.
    /// The type of the data is specific to its source.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgData {

        /// <summary>
        /// Time when the message was sent.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Zero-based number of the road according to the "Information"
        /// message.
        /// </summary>
        public int roadNo;

        /// <summary>
        /// Number of the device that triggered this event.
        /// </summary>
        public int deviceNo;

        /// <summary>
        /// Name of the data source.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string name;

        /// <summary>
        /// Name of the data type used for the data payload.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
        public string type;

        /// <summary>
        /// Length of the following message part in bytes.
        /// </summary>
        public int dataSize;
    }
}