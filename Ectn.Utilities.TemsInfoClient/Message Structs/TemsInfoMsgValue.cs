using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Value" message is sent by the TEMS system when a runtime value
    /// changes its value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgValue {

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
        /// Name of the value.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string name;

        /// <summary>
        /// Data type of the value.
        /// </summary>
        public TemsInfoDataType type;
    }
}