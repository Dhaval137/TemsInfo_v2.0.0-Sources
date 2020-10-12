namespace Ectn.Utilities.TemsInfo {

    public enum TemsInfoMovingDirection : byte {

        Unknown,

        /// <summary>
        /// The vehicle is moving towards the gantry.
        /// </summary>
        Towards = 1,

        /// <summary>
        /// The vehicle is moving away from the gantry.
        /// </summary>
        Away = 2,
    }
}