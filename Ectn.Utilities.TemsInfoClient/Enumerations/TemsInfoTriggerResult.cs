namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// Reasons for the stop of the measurement of an object.
    /// </summary>
    public enum TemsInfoTriggerResult : int {

        Unknown = 0,

        Normal = 1,

        /// <summary>
        /// The object belongs to another larger object and will no longer
        /// be regarded as a single object. No vehicle result will be sent
        /// for this Object ID.
        /// </summary>
        Merged = 2,

        /// <summary>
        /// The object turned out to be a phantom of some kind. No vehicle
        /// result will be sent for this Object ID.
        /// </summary>
        Discarded = 3,
    }
}