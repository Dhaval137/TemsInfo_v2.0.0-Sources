using System;
using System.ComponentModel;
using System.Windows;

namespace Ectn.Utilities.TemsInfoClientGui {

    public class PropertyChangedBase : INotifyPropertyChanged {

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        protected virtual void OnPropertyChanged(string propertyName) {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }));
        }
    }
}