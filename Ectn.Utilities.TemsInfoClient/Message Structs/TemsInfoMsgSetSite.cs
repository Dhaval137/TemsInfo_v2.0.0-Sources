using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "SetSiteConfiguration" message is sent by the client at any time
    /// either to set and start a new site configuration or to stop the running
    /// site configuration.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgSetSite {

        /// <summary>
        /// Length of the following message part in bytes.
        /// "0" stops the currently running configuration.
        /// </summary>
        public int siteConfigurationSize;
    }
}