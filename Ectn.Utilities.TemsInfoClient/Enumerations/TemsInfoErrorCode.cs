namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// Specifies the type of error that caused the initialization to fail.
    /// </summary>
    public enum TemsInfoErrorCode : int {

        Unknown = 0,

        /// <summary>
        /// The server doesn't support the interface version of the client.
        /// </summary>
        VersionMismatch = 1,

        /// <summary>
        /// The server has already the maximal number of clients connected.
        /// </summary>
        TooManyClients = 2,

        /// <summary>
        /// The server is configured to allow connection only from a
        /// specified IP address.
        /// </summary>
        AccessDenied = 3,

        /// <summary>
        /// The server already has an active connection for the specified
        /// client identifier.
        /// </summary>
        AlreadyConnected = 4,
    }
}