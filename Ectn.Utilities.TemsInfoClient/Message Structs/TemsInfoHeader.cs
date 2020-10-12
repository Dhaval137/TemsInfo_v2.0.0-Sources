using System.Runtime.InteropServices;

namespace Ectn.Utilities.TemsInfo {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TemsInfoHeader {

        #region Constant Values

        internal const byte START_MARKER = 0x02;

        #endregion Constant Values

        #region Fields

        public readonly byte startMarker;

        public byte sequenceNo;

        public TemsInfoMessageType messageType;

        public uint messageLength;

        #endregion Fields

        public TemsInfoHeader(byte sequenceNo, TemsInfoMessageType type, uint messageLength) {
            startMarker = START_MARKER;
            this.sequenceNo = sequenceNo;
            messageType = type;
            this.messageLength = messageLength;
        }
    }
}