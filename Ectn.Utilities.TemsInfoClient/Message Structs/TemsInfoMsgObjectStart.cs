using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    /// <summary>
    /// The "Object Start" message informs about the start of the measurement
    /// of an ob-ject. For every "Object Start" message there is a
    /// corresponding "Object Stop" message with the same Object ID.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoMsgObjectStart {

        /// <summary>
        /// Time when the message was sent.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Time when the event actually took place.
        /// 
        /// The start of the object cannot be safely detected with
        /// only one rotation of the laser scanner. When the detection
        /// is confirmed this value is the time of the first data that
        /// was assigned to the vehicle.
        /// </summary>
        public long timestampEvent;

        /// <summary>
        /// Unique ID of the vehicle.
        /// 
        /// IDs are increasing but they do not necessarily start at 0
        /// and certain values may be left out.
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
        /// Minimal X-coordinate [m] of the object calculated from the center
        /// of the lane.
        /// 
        /// Since the recording of the object has just started this value is
        /// likely to be very inaccurate.
        /// </summary>
        public float startX;

        /// <summary>
        /// Maximal X-coordinate [m] of the object calculated from the center
        /// of the lane.
        /// 
        /// Since the recording of the object has just started this value is
        /// likely to be very inaccurate.
        /// </summary>
        public float stopX;
    }
}