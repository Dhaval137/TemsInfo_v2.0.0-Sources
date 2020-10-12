namespace Ectn.Utilities.TemsInfo {

    public enum TemsInfoVehicleDirection : byte {

        /// <summary>
        /// The vehicle was moving along the normal direction of its lane.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The vehicle was moving reverse to the normal direction of its lane.
        /// </summary>
        Reverse = 1,
    }
}