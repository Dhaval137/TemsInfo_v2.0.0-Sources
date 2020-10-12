using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Heartbeat" message is sent to inform the receiver that the
    /// TEMS Information Interface is (still) working.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgHeartbeat {

        /// <summary>
        /// The first "Heartbeat" message has sequence number 0 and is sent
        /// after the "Configuration" message.
        /// 
        /// The sequence number is increased by one with every "Heartbeat"
        /// message. Should the counter reach <seealso cref="uint.MaxValue"/>
        /// the next sequence number is 0.
        /// </summary>
        public uint sequenceNo;
    }
}