using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ectn.Utilities.TemsInfoClientGui {

    public static class ScrollHelper {

        public static readonly DependencyProperty AutoScrollProperty =
             DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ListBox),
             new PropertyMetadata(false));

        public static readonly DependencyProperty AutoScrollHandlerProperty =
            DependencyProperty.RegisterAttached("AutoScrollHandler", typeof(AutoScrollHandler), typeof(ListBox));

        public static bool GetAutoScroll(ListBox instance) {
            return (bool)instance.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(ListBox instance, bool value) {
            AutoScrollHandler oldHandler = (AutoScrollHandler)instance.GetValue(AutoScrollHandlerProperty);
            if (oldHandler != null) {
                oldHandler.Dispose();
                instance.SetValue(AutoScrollHandlerProperty, null);
            }
            instance.SetValue(AutoScrollProperty, value);
            if (value) {
                instance.SetValue(AutoScrollHandlerProperty, new AutoScrollHandler(instance));
            }
        }
    }

    public class AutoScrollHandler : DependencyObject, IDisposable {

        #region Props

        public IEnumerable ItemsSource {
            get {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
            typeof(AutoScrollHandler), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
                new PropertyChangedCallback(ItemsSourcePropertyChanged)));

        #endregion Props

        #region Fields

        private ListBox target;

        #endregion Fields

        public AutoScrollHandler(ListBox target) {
            this.target = target;
            Binding binding = new Binding("ItemsSource");
            binding.Source = target;
            BindingOperations.SetBinding(this, ItemsSourceProperty, binding);
        }

        #region Public Access

        public void Dispose() {
            BindingOperations.ClearBinding(this, ItemsSourceProperty);
        }

        #endregion Public Access

        #region Utils

        private static void ItemsSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((AutoScrollHandler)o).ItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void ItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
            INotifyCollectionChanged collection = oldValue as INotifyCollectionChanged;
            if (collection != null) {
                collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(Collection_CollectionChanged);
            }
            collection = newValue as INotifyCollectionChanged;
            if (collection != null) {
                collection.CollectionChanged += new NotifyCollectionChangedEventHandler(Collection_CollectionChanged);
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems == null || e.NewItems.Count < 1) {
                return;
            }
            if (target.SelectedIndex >= 0 && target.SelectedIndex < e.NewStartingIndex - 1) {
                return;
            }
            object lastElement = e.NewItems[e.NewItems.Count - 1];
            target.ScrollIntoView(lastElement);
            target.SelectedItem = lastElement;
        }

        #endregion Utils
    }
}