using System.Xml.Serialization;

namespace Ectn.Utilities.TemsInfo {

    public partial class TemsInfoRecoveryBehavior {

        #region Props

        /// <summary>
        /// Maximal number of messages the TEMS Info server should buffer when
        /// the connection is lost.
        /// 
        /// Default value: 300
        /// </summary>
        [XmlElement]
        public int? MessageCount { get; set; } = 300;

        /// <summary>
        /// Maximal age (seconds) of messages the TEMS Info server should
        /// buffer when the connection is lost.
        /// 
        /// Default value: 300 seconds
        /// </summary>
        [XmlElement]
        public float? MessageAge { get; set; } = 300;

        /// <summary>
        /// Only messages that pass through this filter are placed in a buffer
        /// when the connection is lost.
        /// 
        /// Default value: No message is buffered.
        /// </summary>
        [XmlElement]
        public TemsInfoMessageFilter MessageFilter { get; set; }
        
        #endregion Props

        /// <summary>
        /// When property MessageFilter is not NULL the value is returned. Otherwise an empty (pass nothing) TemsInfoMessageFilter instance is assigned to property MessageFilter first.
        /// </summary>
        /// <returns></returns>
        public TemsInfoMessageFilter GetMessageFilter() {
            if (MessageFilter == null) {
                MessageFilter = new TemsInfoMessageFilter();
            }
            return MessageFilter;
        }
    }
}