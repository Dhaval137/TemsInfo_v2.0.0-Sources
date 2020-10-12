using System.IO;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfo {

    public abstract partial class AbstractMessage {

        #region Props

        public abstract TemsInfoMessageType MessageType {
            get;
        }

        #endregion Props

        public AbstractMessage(byte[] content) {
            DecodeContent(content);
        }

        #region Utils

        protected abstract void DecodeContent(byte[] content);

        #endregion Utils
    }
}