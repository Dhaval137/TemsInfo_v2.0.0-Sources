using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Object Stop" message is sent when the measurement of an object is
    /// finished. A corresponding "Object Start" message with the same Object
    /// ID was sent earlier.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgObjectStop {

        /// <summary>
        /// Time when the message was sent.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Time when the event actually took place.
        /// 
        /// The stop of the object cannot be safely detected with only one
        /// rotation of the laser scanner. When the detection is confirmed
        /// this value is the time of the last data that was assigned to
        /// the vehicle.
        /// </summary>
        public long timestampEvent;

        /// <summary>
        /// Unique ID of the vehicle.
        /// 
        /// IDs are increasing but they do not necessarily start at 0 and
        /// certain values may be left out.
        /// </summary>
        public long objectId;

        /// <summary>
        /// Zero-based number of the road according to the "Information"
        /// message.
        /// </summary>
        public int roadNo;

        /// <summary>
        /// Zero-based number of the device that triggered this event.
        /// </summary>
        public int deviceNo;

        /// <summary>
        /// Reason for the stop of the measurement of this object.
        /// </summary>
        public TemsInfoTriggerResult result;

        /// <summary>
        /// Minimal X-coordinate [m] of the object in meters calculated from
        /// the center of the lane.
        /// 
        /// The difference between Start X and Stop X will be close to the
        /// "Width" in the "Vehicle Result" message. The difference is caused
        /// by the filters that are applied to the 3D-model to improve the
        /// measurement results.
        /// </summary>
        public float startX;

        /// <summary>
        /// Maximal X-coordinate [m] of the object calculated from the center
        /// of the lane.
        /// 
        /// The difference between Start X and Stop X will be close to the
        /// "Width" in the "Vehicle Result" message. The difference is caused
        /// by the filters that are applied to the 3D-model to improve the
        /// measurement results.
        /// </summary>
        public float stopX;

        /// <summary>
        /// Number of IDs of "Object Location" messages that belong to this
        /// vehicle.
        /// </summary>
        public int numberOfTrackedFrontTriggerIds;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgObjectStop_TrackedFrontTriggerId {

        /// <summary>
        /// "Unique ID" of an "Object Location" message that was triggered
        /// because of the same vehicle.
        /// 
        /// These IDs are always negative numbers.
        /// </summary>
        public long id;
    }
}