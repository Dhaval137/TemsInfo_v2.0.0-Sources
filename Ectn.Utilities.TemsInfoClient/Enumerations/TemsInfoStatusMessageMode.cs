namespace Ectn.Utilities.TemsInfo {

    public enum TemsInfoStatusMessageMode : ushort {

        /// <summary>
        /// No "Status" messages are sent.
        /// </summary>
        None = 0,

        /// <summary>
        /// The "Status" message is sent immediately after a change to the
        /// status.
        /// </summary>
        Change = 1,

        /// <summary>
        /// The "Status" message is sent periodically (see
        /// "StatusMessageInterval").
        /// </summary>
        Periodically = 2,

        /// <summary>
        /// The "Status" message is sent periodically (see
        /// "StatusMessageInterval") and additionally immediately after a
        /// change to the status.
        /// </summary>
        Both = 3,
    }
}