using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Status" message contains information about the current state of
    /// the TEMS system and all its sub-systems.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgStatus {

        /// <summary>
        /// Time when the message was sent.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Indicates the global health state of the TEMS system.
        /// </summary>
        public TemsInfoHealthState systemState;

        /// <summary>
        /// Length of the following message part in bytes.
        /// </summary>
        public int statusSize;
    }
}