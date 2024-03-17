using Google.Protobuf;
using System.Net.WebSockets;

namespace Backend
{
    public class Client
    {
        public enum RegistrationResponse
        {
            ClientIsNotConnectedToTheServer,
            SubmittedUserRegistrationNotValid,
            FailedToFormRegisterRequest,
            FailedToGetServerResponse,
            ServerRegisterResponseInvalid,
            ServerDenyReasonUsernameTaken,
            ServerDenyReasonUsernameInvalidSyntax,
            ServerDenyReasonUsernameInvalidLength,
            ServerDenyReasonPasswordIsBlank,
            ServerDenyReasonPasswordIsWeak,
            ServerDenyReasonUnknown,
            ServerDenyInterpretationUnknown,
            NewUserRegistrationSucceeded,
            NewUserRegistrationFailed
        }

        public enum LoginResponse
        {
            UserCredentialsNotSet,
            FailedToGetServerLoginResponse,
            FailedToGetServerResponse,
            ServerLoginResponseInvalid,
            ServerDenyReasonInvalidUsername,
            ServerDenyReasonInvalidPassword,
            ServerDenyReasonUnknown,
            ServerDenyInterpretationUnknown,
            UserLoginSucceeded,
            UserLoginFailed
        }

        public sealed class Credentials
        {
            private string _username;
            private string _password;
            private string _authToken;

            public string Username { get => _username; set => _username = value; }
            public string Password { get => _password; set => _password = value; }
            public string AuthToken { get => _authToken; set => _authToken = value; }
            public bool Filled 
            { 
                get
                {
                    return !(string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_authToken));
                }
            }

            public bool CredentialsFilled
            {
                get
                {
                    return !(string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password));
                }
            }

            public Credentials()
            {
                _username = string.Empty;
                _password = string.Empty;
                _authToken = string.Empty;
            }
            public Credentials(string username, string password, string authToken)
            {
                _username = username;
                _password = password;
                _authToken = authToken;
            }
        }

        // Server URL (DO NOT TOUCH)
        const string _serverUrl = "wss://ws.project-flare.net/";

        // Client contacts the server only through this channel
        private ClientWebSocket _webSocket;

        // Required to specify a maximum time period for contact tasks
        private CancellationTokenSource _ctSource;

        // Whether the client has successfully connected to the server
        private bool _connected;

        // Storing user credentials (loaded or new registration)
        private Credentials _usrCredentials;

        public bool IsConnected { get => _connected; }
        public Credentials UserCredentials
        {
            get => _usrCredentials;
            set
            {
                if (value.CredentialsFilled)
                    _usrCredentials = value;
            }
        }

        public Client()
        {
            _webSocket = new ClientWebSocket();
            // [FOR EDIT]
            _ctSource = new CancellationTokenSource();
            _ctSource.CancelAfter(TimeSpan.FromSeconds(60));
            _connected = false;
            // TODO - import if the user is already registered
            _usrCredentials = new Credentials();
        }

        public async Task ConnectToServer()
        {
            // Connect to server
            await _webSocket.ConnectAsync(new Uri(_serverUrl), _ctSource.Token);

            // 1KB buffer
            const int ONE_KILOBYTE = 1024;
            byte[] buffer = new byte[ONE_KILOBYTE];

            // Get server response
            WebSocketReceiveResult rs = await _webSocket.ReceiveAsync(buffer, _ctSource.Token);

            // Check if received message bytes do not exceed the size of the buffer
            int BYTES_RECEIVED = rs.Count;
            if (BYTES_RECEIVED > ONE_KILOBYTE)
                return;

            // Start decoding message from the buffers start
            const int OFFSET = 0;
            // Check if the server response is greeting
            Flare.ServerMessage response = Flare.ServerMessage.Parser.ParseFrom(buffer, OFFSET, BYTES_RECEIVED);
            _connected = response.ServerMessageTypeCase.Equals(Flare.ServerMessage.ServerMessageTypeOneofCase.Hello);
        }

        public async Task<RegistrationResponse> RegisterToServer(UserRegistration registration)
        {
            if (!_connected)
                return RegistrationResponse.ClientIsNotConnectedToTheServer;

            if (!registration.IsValid)
                return RegistrationResponse.SubmittedUserRegistrationNotValid;

            var registerRequest = registration.FormRegistrationRequest();

            if (registerRequest is null)
                return RegistrationResponse.FailedToFormRegisterRequest;

            // Send request to server
            var clientMessage = new Flare.ClientMessage();
            clientMessage.RegisterRequest = registerRequest;
            byte[] buffer = clientMessage.ToByteArray();
            const bool END_OF_MESSAGE = true;
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, END_OF_MESSAGE, _ctSource.Token);

            // Get server response
            const int KILOBYTE = 1024;
            const int OFFSET = 0;
            buffer = new byte[KILOBYTE];

            WebSocketReceiveResult rs = await _webSocket.ReceiveAsync(buffer, _ctSource.Token);
            int BYTES_RECEIVED = rs.Count;
            if (BYTES_RECEIVED > KILOBYTE)
                return RegistrationResponse.FailedToGetServerResponse;

            Flare.ServerMessage serverMessage = Flare.ServerMessage.Parser.ParseFrom(buffer, OFFSET, BYTES_RECEIVED);

            if (serverMessage.ServerMessageTypeCase != Flare.ServerMessage.ServerMessageTypeOneofCase.RegisterResponse)
                return RegistrationResponse.ServerRegisterResponseInvalid;

            Flare.RegisterResponse registerResponse = serverMessage.RegisterResponse;

            if (registerResponse.HasDenyReason)
            {
                switch (registerResponse.DenyReason)
                {
                    case Flare.RegisterResponse.Types.RegisterDenyReason.RdrUsernameTaken:
                        return RegistrationResponse.ServerDenyReasonUsernameTaken;
                    case Flare.RegisterResponse.Types.RegisterDenyReason.RdrUsernameInvalidSymbols:
                        return RegistrationResponse.ServerDenyReasonUsernameInvalidSyntax;
                    case Flare.RegisterResponse.Types.RegisterDenyReason.RdrUsernameInvalidLength:
                        return RegistrationResponse.ServerDenyReasonUsernameInvalidLength;
                    case Flare.RegisterResponse.Types.RegisterDenyReason.RdrPasswordBlank:
                        return RegistrationResponse.ServerDenyReasonPasswordIsBlank;
                    case Flare.RegisterResponse.Types.RegisterDenyReason.RdrPasswordWeak:
                        return RegistrationResponse.ServerDenyReasonPasswordIsWeak;
                    case Flare.RegisterResponse.Types.RegisterDenyReason.RdrUnknown:
                        return RegistrationResponse.ServerDenyReasonUnknown;
                    default:
                        return RegistrationResponse.ServerDenyInterpretationUnknown;
                }
            }

            if (registerResponse.HasAuthToken)
            {
                _usrCredentials.Username = registration.Username;
                _usrCredentials.Password = registration.Password;
                _usrCredentials.AuthToken = registerResponse.AuthToken;
                return RegistrationResponse.NewUserRegistrationSucceeded;
            }

            return RegistrationResponse.NewUserRegistrationFailed;
        }

        public async Task<LoginResponse> LoginToServer()
        {
            if (!_usrCredentials.CredentialsFilled)
                return LoginResponse.UserCredentialsNotSet;

            var loginRequest = new Flare.LoginRequest();
            loginRequest.Username = _usrCredentials.Username;
            loginRequest.Password = _usrCredentials.Password;

            var clientMessage = new Flare.ClientMessage();
            clientMessage.LoginRequest = loginRequest;
            byte[] buffer = clientMessage.ToByteArray();
            const bool END_OF_MESSAGE = true;

            await _webSocket.SendAsync(buffer, WebSocketMessageType.Binary, END_OF_MESSAGE, _ctSource.Token);

            const int KILOBYTE = 1024;
            const int OFFSET = 0;
            buffer = new byte[KILOBYTE];

            var response = await _webSocket.ReceiveAsync(buffer, _ctSource.Token);

            int responseByteCount = response.Count;
            if (responseByteCount > KILOBYTE)
                return LoginResponse.FailedToGetServerLoginResponse;

            var serverMessage = Flare.ServerMessage.Parser.ParseFrom(buffer, OFFSET, responseByteCount);
            
            if (serverMessage.ServerMessageTypeCase != Flare.ServerMessage.ServerMessageTypeOneofCase.LoginResponse)
                return LoginResponse.ServerLoginResponseInvalid;

            Flare.LoginResponse loginResponse = serverMessage.LoginResponse;

            if (loginResponse.HasDenyReason)
            {
                switch (loginResponse.DenyReason)
                {
                    case Flare.LoginResponse.Types.LoginDenyReason.LdrUsernameInvalid:
                        return LoginResponse.ServerDenyReasonInvalidUsername;
                    case Flare.LoginResponse.Types.LoginDenyReason.LdrPasswordInvalid:
                        return LoginResponse.ServerDenyReasonInvalidPassword;
                    case Flare.LoginResponse.Types.LoginDenyReason.LdrUnknown:
                        return LoginResponse.ServerDenyReasonUnknown;
                    default:
                        return LoginResponse.ServerDenyInterpretationUnknown;
                }
            }

            if (loginResponse.HasAuthToken)
            {
                _usrCredentials.AuthToken = loginResponse.AuthToken;
                return LoginResponse.UserLoginSucceeded;
            }

            return LoginResponse.UserLoginFailed;
        }
    }
}
