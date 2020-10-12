namespace Ectn.Utilities.TemsInfo {

    public enum TemsInfoBinaryDataType : byte {

        /// <summary>
        /// Unspecified data type
        /// </summary>
        Unknown = 0xFF,

        /// <summary>
        /// A UTF-8 encoded XML document with all available measurement values
        /// for the vehicle.
        /// </summary>
        VehicleXml = 0,

        /// <summary>
        /// ZIP-archive containing the Vehicle XML and all associated binary
        /// components (3D-Modell, ...).
        /// </summary>
        Vehicle = 1,

        /// <summary>
        /// Point cloud data used for client side visualization of the recorded
        /// vehicle.
        /// </summary>
        PointCloud = 2,

        /// <summary>
        /// Timed point cloud data used for client side visualization or
        /// rearrangement of the recorded vehicle.
        /// </summary>
        TimedPointCloud = 3,

        /// <summary>
        /// Complete JPEG image as it would be stored in a file.
        /// </summary>
        Jpeg = 4,
    }
}