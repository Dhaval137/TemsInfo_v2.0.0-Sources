using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// If the server cannot accept the connection from a client it responds
    /// with this message and closes the connection.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgError {

        /// <summary>
        /// Specifies the type of error that caused the initialization to fail.
        /// </summary>
        public TemsInfoErrorCode errorCode;

        /// <summary>
        /// Textual description of the error.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
        public string errorMessage;
    }
}