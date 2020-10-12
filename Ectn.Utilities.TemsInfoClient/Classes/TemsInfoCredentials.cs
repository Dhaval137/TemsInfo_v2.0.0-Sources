using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ectn.Utilities.TemsInfo {

    public class TemsInfoCredentials {

        #region Props

        /// <summary>
        /// User level to connect.
        /// 
        /// Default value: <seealso cref="TemsInfoUserLevel.Operator"/>
        /// </summary>
        [XmlElement]
        public TemsInfoUserLevel UserLevel {
            get;
            set;
        }

        /// <summary>
        /// Password matching the <see cref="UserLevel"/>.
        /// 
        /// Default value: <c>string.empty</c>
        /// </summary>
        [XmlElement]
        public string Password {
            get;
            set;
        }

        #endregion Props

        /// <summary>
        /// Creates a <c>TemsInfoCredentials</c> with default user
        /// <seealso cref="TemsInfoUserLevel.Operator"/> and empty password.
        /// </summary>
        public TemsInfoCredentials() {
            UserLevel = TemsInfoUserLevel.Operator;
            Password = string.Empty;
        }
    }
}