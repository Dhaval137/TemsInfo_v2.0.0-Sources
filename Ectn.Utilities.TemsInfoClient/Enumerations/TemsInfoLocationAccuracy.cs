namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// Assessment of the accuracy of a location value.
    /// </summary>
    public enum TemsInfoLocationAccuracy : byte {

        Unknown = 0,

        /// <summary>
        /// The vehicle was tracked while it crossed the trigger line.
        /// </summary>
        Accurate = 1,

        /// <summary>
        /// The time was extrapolated from partial tracking or speed
        /// measurement data. In this case the message is not sent in
        /// real-time but too late (moving towards) or too early (moving
        /// away).
        /// </summary>
        Estimated = 2,

        /// <summary>
        /// The algorithm decided that the tracked object was not a
        /// vehicle at all.
        /// </summary>
        Phantom = 3,
    }
}