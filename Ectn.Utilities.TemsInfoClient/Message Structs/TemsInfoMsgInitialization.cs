using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// Before the server starts to send any messages it waits for the
    /// "Initialization" message from the client. 
    /// 
    /// Using this message the client tells the server for which version of the
    /// interface specification it was created. The server will then only send
    /// messages compatible with this version of the specification. 
    /// 
    /// The structure of this message will remain unchanged for all future
    /// versions of this specification.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgInitialization {

        /// <summary>
        /// Version number of the communication protocol.
        /// </summary>
        public TemsInfoVersion clientVersion;
    }
}