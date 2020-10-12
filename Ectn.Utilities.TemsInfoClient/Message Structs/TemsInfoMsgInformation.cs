using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// When the server has successfully received the "Initialization" message,
    /// it sends the "Information" message immediately as a response.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgInformation_Part1 {

        /// <summary>
        /// Current version number of the server (this version might be newer
        /// than that of the client!).
        /// </summary>
        public TemsInfoVersion serverVersion;

        /// <summary>
        /// Length of the following message part in bytes.
        /// </summary>
        public int recorderConfigurationSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgInformation_Part2 {

        /// <summary>
        /// Length of the following message part in bytes.
        /// </summary>
        public int siteConfigurationSize;
    }
}