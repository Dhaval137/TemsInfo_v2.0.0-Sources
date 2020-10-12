using System;
using System.Text;

namespace Ectn.Utilities.TemsInfo {

    public partial class ErrorMessage : AbstractMessage {

        #region Props

        public override TemsInfoMessageType MessageType {
            get {
                return TemsInfoMessageType.Error;
            }
        }

        public TemsInfoErrorCode Code {
            get;
            private set;
        }

        public string Message {
            get;
            private set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>ErrorMessage</c> from received bytes.
        /// </summary>
        /// <param name="content">Received bytes.</param>
        public ErrorMessage(byte[] content)
            : base(content) {
        }

        #region Overridden

        protected override void DecodeContent(byte[] content) {
            TemsInfoMsgError msg = ByteArrayHelper.GetMessage<TemsInfoMsgError>(content);
            this.Code = msg.errorCode;
            this.Message = msg.errorMessage;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TEMS Info Message of type \"{0}\":{1}", MessageType, Environment.NewLine);
            sb.AppendFormat("   Code = {0}", Code);
            sb.AppendLine();
            sb.AppendFormat("   Message = {0}", Message);
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Overridden
    }
}