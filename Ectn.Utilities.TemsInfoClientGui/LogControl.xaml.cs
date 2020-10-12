using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ectn.Utilities.TemsInfoClientGui {

    public partial class LogControl : UserControl {

        #region Props

        public ObservableCollection<LogEntry> LogMessages {
            get {
                return (ObservableCollection<LogEntry>)GetValue(LogMessagesProperty);
            }
            set {
                SetValue(LogMessagesProperty, value);
            }
        }

        public static readonly DependencyProperty LogMessagesProperty =
            DependencyProperty.Register("LogMessages", typeof(ObservableCollection<LogEntry>), typeof(LogControl));

        #endregion Props

        public LogControl() {
            LogMessages = new ObservableCollection<LogEntry>();

            InitializeComponent();
        }

        #region Public Access

        public void AddLogMessage(string message, params object[] args) {
            Dispatcher.BeginInvoke(new Action<string, object[]>((dispMessage, dispArgs) => {
                LogMessages.Add(new LogEntry() {
                    Timestamp = DateTime.Now,
                    Message = string.Format(dispMessage, dispArgs),
                });
            }), message, args);
        }

        internal void Clear() {
            LogMessages.Clear();
        }

        #endregion Public Access
    }
}