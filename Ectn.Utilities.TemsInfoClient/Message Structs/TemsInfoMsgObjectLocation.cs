using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Object Location" message notifies the receiver that an object
    /// crossed a trigger line on the Z-axis.
    /// 
    /// The location of each trigger line is configured as part of the TEMS
    /// setup.
    /// 
    /// The TEMS system will either send all "Object Location" messages (one
    /// for each tirgger line) or none at all.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgObjectLocation {

        /// <summary>
        /// Time when the message was sent.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Time when the event actually took place or will take place or will
        /// take place.
        /// 
        /// In case the tracking fails, the message is generated after
        /// (moving towards) or before (moving away) the real event.
        /// </summary>
        public long timestampEvent;

        /// <summary>
        /// If "Moving Direction" is "Away" this is the same ID as in the
        /// corresponding "Object Stop" message of the same vehicle. In this
        /// case the ID is larger than or equal to zero.
        /// 
        /// If "Moving Direction" is "Towards" this is a unique ID of this
        /// trigger event. The "Object Stop" message has a list "Tracked Front
        /// Triggers" that contains all of the IDs that belong to the same
        /// vehicle. The ID is a negative number in this case.
        /// </summary>
        public long objectId;

        /// <summary>
        /// Zero-based number of the road according to the
        /// <seealso cref="TemsInfoMsgInformation"/>.
        /// </summary>
        public int roadNo;

        /// <summary>
        /// Zero-based number of the device that triggered this event.
        /// </summary>
        public int deviceNo;

        /// <summary>
        /// Location [m] of the trigger line on the Z-axis.
        /// 
        /// This value corresponds to the value in the TEMS configuration.
        /// </summary>
        public float triggerLocation;

        /// <summary>
        /// Current location [m] of the object on the Z-axis.
        /// 
        /// This value is the location of the vehicle at the time when it
        /// was possible to decide that the object crossed the trigger line.
        /// Therefore a certain variation is possible.
        /// </summary>
        public float location;

        /// <summary>
        /// Either the front or the rear of the vehicle is at the specified
        /// location depending on whether the object moves toward the gantry
        /// (front) or away from it (rear).
        /// </summary>
        public TemsInfoMovingDirection movingDirection;

        /// <summary>
        /// Assessment of the accuracy of the location value.
        /// </summary>
        public TemsInfoLocationAccuracy accuracy;
    }
}