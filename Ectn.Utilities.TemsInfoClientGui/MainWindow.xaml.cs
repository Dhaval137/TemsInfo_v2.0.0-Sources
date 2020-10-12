using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Ectn.Utilities.TemsInfo;
using Microsoft.Win32;

namespace Ectn.Utilities.TemsInfoClientGui {

    public partial class MainWindow : Window {

        #region Constant Values

        private const int MAX_DISPLAYED_MESSAGES = 1000;
        private const string INDENT_CHARS = "    ";
        private const string CONFIGURATION_FILE_FILTER = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";

        private const int DEFAULT_PORT_NUMBER = 54345;

        #endregion Constant Values

        #region Props

        public ObservableCollection<MessageInfo> ReceivedMessages {
            get {
                return (ObservableCollection<MessageInfo>)GetValue(ReceivedMessagesProperty);
            }
            set {
                SetValue(ReceivedMessagesProperty, value);
            }
        }

        public static readonly DependencyProperty ReceivedMessagesProperty =
            DependencyProperty.Register("ReceivedMessages", typeof(ObservableCollection<MessageInfo>), typeof(MainWindow));


        public bool IsOnline {
            get {
                return (bool)GetValue(IsOnlineProperty);
            }
            set {
                SetValue(IsOnlineProperty, value);
            }
        }

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));


        public bool IsConnected {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(MainWindow));

        #endregion Props

        #region Fields

        private TemsInfoClient client;
        private uint? lastHeartbeatSequenceNo;

        #endregion Fields

        public MainWindow() {
            ReceivedMessages = new ObservableCollection<MessageInfo>();

            InitializeComponent();

        }

        #region Component Events

        #region Window

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            lastHeartbeatSequenceNo = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (client != null && client.State == TemsInfoClient.ConnectionState.Ok) {
                MessageBoxResult result = MessageBox.Show(this, string.Format("TEMS Info Client is still Online!{0}{0}Close anyway?", Environment.NewLine), "Warning ...", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                e.Cancel = result != MessageBoxResult.Yes;
            }
        }

        private void Window_Closed(object sender, EventArgs e) {
            try {
                Properties.Settings.Default.Save();
            } catch {
                // Ignore
            }

            DisposeClient();

            // Delete temporary files
            foreach (MessageInfo msg in ReceivedMessages) {
                msg.DeleteTemporaryDocumentFiles();
            }
        }

        #endregion Window

        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            if (client != null) {
                MessageBoxResult result = MessageBox.Show(this, string.Format("There is still a client running!{0}{0}Would you like to reconnect?", Environment.NewLine), "Warning ...", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (result != MessageBoxResult.Yes) {
                    return;
                }
            }

            int tcpPort;
            if (!int.TryParse(txtConnectionPort.Text, out tcpPort)) {
                tcpPort = DEFAULT_PORT_NUMBER;
                txtConnectionPort.Text = DEFAULT_PORT_NUMBER.ToString();
            }

            StartClient(
                txtConnectionAddress.Text,
                tcpPort,
                txtConnectionId.Text);
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
            DisposeClient();
        }

        private void btnSetSite_Click(object sender, RoutedEventArgs e) {
            if (client != null) {
                OpenFileDialog ofd = new OpenFileDialog() {
                    DefaultExt = "temsXml",
                    Filter = "TEMS Files (*.temsXml)|*.temsXml|All Files (*.*)|*.*"
                };

                if (ofd.ShowDialog() ?? false) {
                    XDocument doc = XDocument.Load(ofd.FileName);
                    client.SetSite(doc);
                }
            }
        }

        private void btnResetSite_Click(object sender, RoutedEventArgs e) {
            if (client != null) {
                client.SetSite(null);
            }
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e) {
            ctrlLog.Clear();
        }

        private void btnClearMessages_Click(object sender, RoutedEventArgs e) {
            while (ReceivedMessages.Count > 0) {
                ReceivedMessages[0].DeleteTemporaryDocumentFiles();
                ReceivedMessages.RemoveAt(0);
            }
        }

        #region Configuration

        private void btnConfigurationLoad_Click(object sender, RoutedEventArgs args) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = CONFIGURATION_FILE_FILTER;
            if (dialog.ShowDialog(this) ?? false) {
                XDocument document = null;
                try {
                    document = XDocument.Load(dialog.FileName);
                } catch (Exception e) {
                    MessageBox.Show(this, string.Format("Failed loading configuration from '{0}' as XML document:{1}{1}{2}", dialog.SafeFileName, Environment.NewLine, e.Message), "Error ...", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string[] messages;
                if (!ClientConfiguration.Validate(document, out messages)) {
                    MessageBox.Show(this, string.Format("Validation of configuration file failed:{0}{0}{1}", Environment.NewLine, string.Join("  - ", messages)), "Error ...", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    try {
                        txtConfiguration.Text = ClientConfiguration.Load(dialog.FileName).ToSerializedXml();
                    } catch (Exception e) {
                        MessageBox.Show(this, string.Format("Failed to load configuration from '{0}':{1}{1}{2}", dialog.SafeFileName, Environment.NewLine, e.Message), "Error ...", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnConfigurationSave_Click(object sender, RoutedEventArgs e) {
            ClientConfiguration configuration = GetClientConfiguration();
            if (configuration != null) {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = CONFIGURATION_FILE_FILTER;
                if (dialog.ShowDialog(this) ?? false) {
                    configuration.Save(dialog.FileName);
                }
            }
        }

        private void btnConfigurationReset_Click(object sender, RoutedEventArgs e) {
            ClientConfiguration configuration = new ClientConfiguration() {
                HeartbeatMessageInterval = 1,
                MessageFilter = TemsInfoMessageFilter.CreateAllowAllFilter(),
                StatusMessageMode = TemsInfoStatusMessageMode.Both,
                StatusMessageInterval = 60,
            };
            txtConfiguration.Text = configuration.ToSerializedXml();
        }

        private void btnConfigurationUpdate_Click(object sender, RoutedEventArgs e) {
            UpdateClientConfiguration();
        }

        private void txtConfiguration_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Tab) {
                e.Handled = true;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                    if (txtConfiguration.CaretIndex >= INDENT_CHARS.Length && txtConfiguration.Text.Substring(txtConfiguration.CaretIndex - INDENT_CHARS.Length, INDENT_CHARS.Length) == INDENT_CHARS) {
                        int oldCaretIndex = txtConfiguration.CaretIndex;
                        txtConfiguration.Text = txtConfiguration.Text.Remove(oldCaretIndex - INDENT_CHARS.Length, INDENT_CHARS.Length);
                        txtConfiguration.CaretIndex = oldCaretIndex - INDENT_CHARS.Length;
                    }
                } else {
                    int oldCaretIndex = txtConfiguration.CaretIndex;
                    txtConfiguration.Text = txtConfiguration.Text.Insert(txtConfiguration.CaretIndex, INDENT_CHARS);
                    txtConfiguration.CaretIndex = oldCaretIndex + INDENT_CHARS.Length;
                }
            }
        }

        #endregion Configuration

        private void lstCommunication_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            OpenDocument((e.OriginalSource as FrameworkElement).DataContext as DocumentInfo);
        }

        #endregion Component Events

        #region Utils

        #region File Handling

        private void OpenDocument(DocumentInfo info) {
            if (info != null && info.Document != null) {
                System.Diagnostics.Process.Start(info.GetTemporaryFile());
            }
        }

        #endregion File Handling

        private void StartClient(string address, int port, string identifer) {
            DisposeClient();

            client = new TemsInfoClient(identifer);
            client.DevelopmentDebugging = true;
            client.OnLogMessage += new LogMessageHandler(client_OnLogMessage);
            client.OnConnectionStateChanged += client_OnConnectionStateChanged;
            client.OnMessageReceived += client_OnMessageReceived;

            UpdateClientConfiguration();

            try {
                client.Initialize(address, port, identifer);

                IsConnected = true;
            } catch (Exception e) {
                DisposeClient();

                ctrlLog.AddLogMessage("Failed initializing TEMS Info Client: {0}{1}{2}", e.Message, Environment.NewLine, e);
                MessageBox.Show(this, string.Format("Failed initializing TEMS Info Client: {0}", e.Message), "Error ...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisposeClient() {
            if (client != null) {
                try {
                    client.Dispose();
                } catch (Exception e) {
                    ctrlLog.AddLogMessage("Failed to disconnect: {0}", e);
                } finally {
                    client = null;
                }
            }

            IsConnected = false;
        }

        private ClientConfiguration GetClientConfiguration() {
            XDocument document = null;
            try {
                document = XDocument.Parse(txtConfiguration.Text);
            } catch (Exception e) {
                MessageBox.Show(this, string.Format("Failed loading configuration from editor as XML document:{0}{0}{1}", Environment.NewLine, e.Message), "Error ...", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            string[] messages;
            if (ClientConfiguration.Validate(document, out messages)) {
                return ClientConfiguration.FromSerializedXml(txtConfiguration.Text);
            } else {
                MessageBox.Show(this, string.Format("Validation of configuration failed:{0}{0}{1}", Environment.NewLine, string.Join("  - ", messages)), "Error ...", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void UpdateClientConfiguration() {
            if (client != null) {
                ClientConfiguration configuration = GetClientConfiguration();
                if (configuration != null) {
                    client.ChangeConfiguration(configuration);
                }
            }
        }

        private void client_OnLogMessage(string text, params object[] args) {
            Dispatcher.BeginInvoke((Action)(() => {
                ctrlLog.AddLogMessage(text, args);
            }));
        }

        private void client_OnMessageReceived(AbstractMessage message) {
            MessageInfo info = null;
            switch (message.MessageType) {
                case TemsInfoMessageType.Information:
                    info = new MessageInfo(message as InformationMessage);
                    break;
                case TemsInfoMessageType.Error:
                    info = new MessageInfo(message as ErrorMessage);
                    break;
                case TemsInfoMessageType.Heartbeat:
                    HeartbeatMessage msgHeartbeat = message as HeartbeatMessage;
                    if (lastHeartbeatSequenceNo.HasValue) {
                        uint expectedHeartbeatSequenceNo;
                        if (lastHeartbeatSequenceNo.HasValue && lastHeartbeatSequenceNo.Value < uint.MaxValue) {
                            expectedHeartbeatSequenceNo = lastHeartbeatSequenceNo.Value + 1;
                        } else {
                            expectedHeartbeatSequenceNo = uint.MinValue;
                        }
                        if (expectedHeartbeatSequenceNo != msgHeartbeat.SequenceNo) {
                            ctrlLog.AddLogMessage("Warning: Irregular heart beat sequence number (Last = {0:#,##0}, This = {1:#,##0})", lastHeartbeatSequenceNo, msgHeartbeat.SequenceNo);
                        }
                    }
                    lastHeartbeatSequenceNo = msgHeartbeat.SequenceNo;
                    Dispatcher.BeginInvoke((Action)(() => {
                        txtHeartbeat.Text = string.Format("Heartbeat #: {0:#,##0}", msgHeartbeat.SequenceNo);
                    }));
                    break;
                case TemsInfoMessageType.Vehicle:
                    info = new MessageInfo(message as VehicleMessage);
                    break;
                case TemsInfoMessageType.ObjectStart:
                    info = new MessageInfo(message as ObjectStartMessage);
                    break;
                case TemsInfoMessageType.ObjectStop:
                    info = new MessageInfo(message as ObjectStopMessage);
                    break;
                case TemsInfoMessageType.ObjectLocation:
                    info = new MessageInfo(message as ObjectLocationMessage);
                    break;
                case TemsInfoMessageType.Status:
                    info = new MessageInfo(message as StatusMessage);
                    break;
                case TemsInfoMessageType.Value:
                    info = new MessageInfo(message as ValueMessage);
                    break;
                case TemsInfoMessageType.Data:
                    info = new MessageInfo(message as DataMessage);
                    break;
                case TemsInfoMessageType.Initialization:
                case TemsInfoMessageType.Configuration:
                case TemsInfoMessageType.SetSite:
                default:
                    // Ignore because messages of this type
                    // are never sent by the server
                    break;
            }

            if (info != null) {
                Dispatcher.BeginInvoke((Action)(() => {
                    ReceivedMessages.Add(info);

                    while (ReceivedMessages.Count > MAX_DISPLAYED_MESSAGES) {
                        ReceivedMessages[0].DeleteTemporaryDocumentFiles();
                        ReceivedMessages.RemoveAt(0);
                    }
                }));
            }
        }

        private void client_OnConnectionStateChanged(TemsInfoClient.ConnectionState state) {
            Dispatcher.BeginInvoke((Action)(() => {
                ConnectionState connectionState;
                switch (state) {
                    case TemsInfoClient.ConnectionState.New:
                        connectionState = ConnectionState.New;
                        IsOnline = false;
                        break;
                    case TemsInfoClient.ConnectionState.Initializing:
                        connectionState = ConnectionState.Initializing;
                        IsOnline = false;
                        break;
                    case TemsInfoClient.ConnectionState.Ok:
                        connectionState = ConnectionState.Ok;
                        IsOnline = true;
                        break;
                    case TemsInfoClient.ConnectionState.Error:
                        connectionState = ConnectionState.Error;
                        IsOnline = false;
                        break;
                    default:
                        connectionState = ConnectionState.Unknown;
                        IsOnline = false;
                        break;
                }
                txtConnectionState.Text = string.Format("State: {0}", connectionState);
            }));
        }

        public enum ConnectionState {
            Unknown,
            New,
            Initializing,
            Ok,
            Error,
        }

        #endregion Utils
    }
}