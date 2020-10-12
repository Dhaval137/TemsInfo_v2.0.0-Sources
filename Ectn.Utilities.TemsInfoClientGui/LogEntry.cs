using System;

namespace Ectn.Utilities.TemsInfoClientGui {

    public class LogEntry : PropertyChangedBase {

        #region Props

        public DateTime Timestamp {
            get;
            set;
        }

        public string Message {
            get;
            set;
        }

        #endregion Props
    }
}