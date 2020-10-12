using System;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfo {

    public delegate void LogMessageHandler(string text, params object[] args);

    public class TemsInfoClient : IDisposable {

        #region Constant Values

        private const TemsInfoVersion CLIENT_VERSION = TemsInfoVersion.Current;

        private const int THREAD_ABORT_TIMEOUT = 5000;

        private const int HEADER_LENGTH = 8;

        #endregion Constant Values

        #region Events

        public event LogMessageHandler OnLogMessage;

        public event Action<ConnectionState> OnConnectionStateChanged;

        public event Action<AbstractMessage> OnMessageReceived;

        #endregion Events

        #region Props

        public string Name {
            get;
            private set;
        }

        public string ClientIdentifier {
            get;
            private set;
        }

        public ClientConfiguration Configuration {
            get;
            private set;
        }

        public ConnectionState State {
            get;
            private set;
        }

        public bool DevelopmentDebugging {
            get;
            set;
        }

        #endregion Props

        #region Fields

        private Thread thrdReceiver;
        private Socket socket;

        private byte sequenceNo;

        #endregion Fields

        public TemsInfoClient(string name) {
            Name = name;
            State = ConnectionState.New;
            sequenceNo = byte.MaxValue;

            Configuration = new ClientConfiguration();
        }

        #region Public Access

        /// <summary>
        /// Initializes the network connection and starts the initializing
        /// the communication.
        /// </summary>
        /// <param name="host">IP or name of the machine the server application is running on.</param>
        /// <param name="port">Port the server application is configured to listen for incoming connections.</param>
        /// <param name="clientIdentifier">Identification used to communicate to the server.</param>
        public void Initialize(string host, int port, string clientIdentifier) {
            this.ClientIdentifier = clientIdentifier;
            StartCommunication(host, port);
        }

        /// <summary>
        /// Stops the communication and disposes the connection and any related
        /// resources.
        /// </summary>
        public void Dispose() {
            StopCommunication();
            ChangeState(ConnectionState.New);
        }

        /// <summary>
        /// Changes the interface configuration.
        /// 
        /// If the connection to the server is already established the
        /// configuration is sent to the server immediatly.
        /// </summary>
        /// <param name="configuration">New interface configuration.</param>
        public void ChangeConfiguration(ClientConfiguration configuration) {
            Configuration = configuration;
            LogDebug("Configuration changed");
            if (State == ConnectionState.Ok) {
                SendConfigurationMessage(Configuration);
            }
        }

        public void SetSite(XDocument siteConfiguration) {
            if (State == ConnectionState.Ok) {
                SendSiteConfigurationMessage(siteConfiguration);
            }
        }

        #endregion Public Access

        #region Utils

        #region Communication Handling

        private void StartCommunication(string host, int port) {
            LogDebug("Creating socket connection to {0}:{1} ...", host, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Connect(host, port);
            LogDebug("... socket connected");

            thrdReceiver = new Thread(new ThreadStart(ReceiverRoutine));
            thrdReceiver.Name = "Receiver";
            thrdReceiver.IsBackground = true;
            thrdReceiver.Start();
        }

        private void StopCommunication() {
            if (socket != null) {
                LogDebug("Closing receiver socket");
                socket.Close();
            }

            if (thrdReceiver != null) {
                if (!thrdReceiver.Join(THREAD_ABORT_TIMEOUT)) {
                    LogDebug("Receiver thread didn't terminate gracefully. Abort it now.");
                    thrdReceiver.Abort();
                }
                thrdReceiver = null;
            }

            socket = null;
        }

        private void ReceiverRoutine() {
            
            // Start communication sending initialization message
            TemsInfoMsgInitialization msg = new TemsInfoMsgInitialization();
            msg.clientVersion = CLIENT_VERSION;
            SendMessage(TemsInfoMessageType.Initialization, msg);

            LogDebug("Receiver thread started");

            byte? serverSequenceNo = null;

            ChangeState(ConnectionState.Initializing);
            byte[] bufferHeader = new byte[HEADER_LENGTH];
            TemsInfoHeader header = new TemsInfoHeader();
            try {
                while (socket.Connected) {

                    // Receive header
                    if (!Receive(bufferHeader, 0, bufferHeader.Length)) {
                        break;
                    }
                    header = ByteArrayHelper.GetMessage<TemsInfoHeader>(bufferHeader);

                    // Check sequence number
                    if (serverSequenceNo.HasValue) {
                        byte expectedSequenceNo = (byte)(serverSequenceNo.Value == byte.MaxValue ? 0 : serverSequenceNo.Value + 1);
                        if (header.sequenceNo != expectedSequenceNo) {
                            Log("Seems we lost some messages: Expected sequence number = {0}, Received = {1}", expectedSequenceNo, header.sequenceNo);
                        }
                    }
                    serverSequenceNo = header.sequenceNo;

                    // Receive content
                    byte[] bufferContent = new byte[header.messageLength];
                    if (!Receive(bufferContent, 0, bufferContent.Length)) {
                        break;
                    }

                    LogDebug("Message ({0}) received", header.messageType);

                    HandleReceivedMessage(header.messageType, bufferContent);
                }

                if (socket != null) {
                    socket.Dispose();
                    socket = null;
                }
            } catch (ThreadAbortException) {
                // Ignore
            } catch (Exception e) {
                Log("Error in receiver routine: {0}", e);
                ChangeState(ConnectionState.Error);
            } finally {
                LogDebug("Receiver thread stopped");
            }
        }

        private bool Receive(byte[] buffer, int offset, int length) {
            while (length > 0) {
                int bytesRead = socket.Receive(buffer, offset, length, SocketFlags.None);
                if (bytesRead == 0) {
                    return false;
                }
                length -= bytesRead;
                offset += bytesRead;
            }

            return true;
        }

        #endregion Receiver Handling

        #region Message Handling

        private void HandleReceivedMessage(TemsInfoMessageType type, byte[] content) {
            AbstractMessage message = null;
            switch (type) {
                case TemsInfoMessageType.Information:
                    message = new InformationMessage(content);

                    if (State == ConnectionState.Initializing) {

                        // Server accepted the client
                        // => Send interface configuration
                        SendConfigurationMessage(Configuration);

                        // => We're online
                        ChangeState(ConnectionState.Ok);
                    }
                    break;
                case TemsInfoMessageType.Error:
                    ErrorMessage msgError = new ErrorMessage(content);
                    if (!string.IsNullOrEmpty(msgError.Message)) {
                        ChangeState(ConnectionState.Error, "Received error message with type \"{0}\": {1}", msgError.Code, msgError.Message);
                    } else {
                        ChangeState(ConnectionState.Error, "Received error message with type \"{0}\"!", msgError.Code);
                    }
                    message = msgError;
                    break;
                case TemsInfoMessageType.Heartbeat:
                    message = new HeartbeatMessage(content);
                    break;
                case TemsInfoMessageType.Vehicle:
                    message = new VehicleMessage(content);
                    break;
                case TemsInfoMessageType.ObjectStart:
                    message = new ObjectStartMessage(content);
                    break;
                case TemsInfoMessageType.ObjectStop:
                    message = new ObjectStopMessage(content);
                    break;
                case TemsInfoMessageType.ObjectLocation:
                    message = new ObjectLocationMessage(content);
                    break;
                case TemsInfoMessageType.Status:
                    message = new StatusMessage(content);
                    break;
                case TemsInfoMessageType.Data:
                    message = new DataMessage(content);
                    break;
                case TemsInfoMessageType.Value:
                    message = new ValueMessage(content);
                    break;
                default:
                    Log("Received unexpected message of type \"{0}\" => Ignoring!", type);
                    break;
            }

            if (message != null) {
                OnMessageReceived?.Invoke(message);
            }
        }

        private void SendMessage(TemsInfoMessageType type, object message) {
            byte[] content = ByteArrayHelper.GetByteArray(message);
            SendMessage(type, content);
        }

        private void SendConfigurationMessage(ClientConfiguration configuration) {
            byte[] data = configuration.ToBytes();
            TemsInfoMsgConfiguration msgConfiguration = new TemsInfoMsgConfiguration() {
                clientIdentifier = ClientIdentifier,
                clientConfigurationSize = data.Length,
            };
            byte[] content = ByteArrayHelper.GetByteArray(msgConfiguration);
            byte[] composed = new byte[content.Length + data.Length];
            content.CopyTo(composed, 0);
            data.CopyTo(composed, content.Length);
            SendMessage(TemsInfoMessageType.Configuration, composed);
        }

        private void SendSiteConfigurationMessage(XDocument siteConfiguration) {
            byte[] data = ByteArrayHelper.GetBytes(siteConfiguration);
            TemsInfoMsgSetSite msgSetConfig = new TemsInfoMsgSetSite() {
                siteConfigurationSize = data.Length,
            };
            byte[] content = ByteArrayHelper.GetByteArray(msgSetConfig);
            byte[] composed = new byte[content.Length + data.Length];
            content.CopyTo(composed, 0);
            data.CopyTo(composed, content.Length);
            SendMessage(TemsInfoMessageType.SetSite, composed);
        }

        /// <summary>
        /// Sends given message handling the sequence number.
        /// </summary>
        /// <param name="type">Type of the message.</param>
        /// <param name="message">Message payload.</param>
        /// <returns><c>True</c> if the message was sent successfully, otherwise <c>False</c>.</returns>
        private bool SendMessage(TemsInfoMessageType type, byte[] message) {
            bool result;

            if (sequenceNo < byte.MaxValue) {
                sequenceNo += 1;
            } else {
                sequenceNo = byte.MinValue;
            }

            if (State != ConnectionState.Error) {
                try {
                    Socket localSocket = this.socket;
                    if (localSocket != null && localSocket.Connected) {
                        LogDebug("Sending message ({0}) ...", type);

                        byte[] header = ByteArrayHelper.GetByteArray(new TemsInfoHeader(sequenceNo, type, (uint)message.Length));
                        byte[] composed = new byte[header.Length + message.Length];
                        header.CopyTo(composed, 0);
                        message.CopyTo(composed, header.Length);

                        localSocket.Send(composed);
                        LogDebug("... message sent");
                        result = true;
                    } else if (localSocket == null) {
                        ChangeState(ConnectionState.Error, "Failed sending message: Socket is NULL!");
                        result = false;
                    } else {
                        ChangeState(ConnectionState.Error, "Failed sending message: Socket is not connected!");
                        result = false;
                    }
                } catch (Exception e) {
                    ChangeState(ConnectionState.Error, "Failed sending message: {0}{1}{2}", e.Message, Environment.NewLine, e);
                    result = false;
                }
            } else {
                result = false;
            }

            if (!result) {
                if (sequenceNo > byte.MinValue) {
                    sequenceNo -= 1;
                } else {
                    sequenceNo = byte.MaxValue;
                }
            }

            return result;
        }

        #endregion Message Handling

        #region State Handling

        protected void ChangeState(ConnectionState state) {
            ChangeState(state, null);
        }

        protected void ChangeState(ConnectionState state, string message, params object[] args) {
            if (state != State) {
                if (!string.IsNullOrEmpty(message)) {
                    Log("Changed connection state from \"{0}\" to \"{1}\": {2}", State, state, string.Format(message, args));
                } else {
                    Log("Changed connection state from \"{0}\" to \"{1}\"", State, state);
                }
                State = state;
                if (OnConnectionStateChanged != null) {
                    OnConnectionStateChanged(State);
                }
            }
        }

        public enum ConnectionState {
            New,
            Initializing,
            Ok,
            Error,
        }

        #endregion State Handling

        #region Log

        protected void Log(string message, params object[] args) {
            if (OnLogMessage != null) {
                OnLogMessage(message, args);
            }
        }

        protected void LogDebug(string message, params object[] args) {
            if (DevelopmentDebugging) {
                Log(message, args);
            }
        }

        #endregion Log

        #endregion Utils
    }
}