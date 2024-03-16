using Flare;
using Google.Protobuf;
using System.Net.WebSockets;
using static Flare.RegisterResponse;
using static Flare.ServerMessage;

namespace Backend
{
    public enum RegistrationResponse
    {
        FormingRegisterRequestFailed,
        RegisterRequestSendingFailed,
        SessionKeyIssueFailed,
        SessionKeyIssueSuccessful,
        UsernameIsTaken,
        UsernameIsInvalid,
        UsernameInvalidLength,
        PasswordIsBlank,
        PasswordIsWeak,
        Unknown,
    }

    public enum AuthenticationResponse
    {
        ClientCredentialsNotValid,
        AuthRequestReceiveFailed,
        UserIsInvalidServerDeny,
        PasswordIsInvalidServerDeny,
        AuthFailedServerDeny,
        ClientAuthSuccess,
        ClientAuthFail
    }

    public readonly struct ClientCredentials
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public ClientCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public override string ToString()
        {
            if (Username is null && Password is null)
                return "NONE";
            return $"{Username} | {Password}";
        }
    }

    // use new client object withing using brackets
    public class Client : IDisposable
    {
        private ClientWebSocket _ws;
        private bool _connected;
        public bool ConnectedToServer 
        { 
            get => _connected; 
        }
        private string _serverUrl;
        public string ServerUrl 
        { 
            get => _serverUrl; 
            set
            {
                // TODO: better check validity of the url
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                {
                    _serverUrl = value;
                }
            }
        }
        private string _sessionKey;
        public string SessionKey { get => _sessionKey; }
        private ClientCredentials _credentials;
        public ClientCredentials Credentials 
        { 
            get => _credentials;
            set
            {
                UsernameValidity usernameValidity = UserRegistration.ValidifyUsername(value.Username);
                PasswordStrength passwordStrength = UserRegistration.EvaluatePassword(value.Password);
                
                if (usernameValidity.Equals(UsernameValidity.Correct) 
                    && (passwordStrength.Equals(PasswordStrength.Good) || passwordStrength.Equals(PasswordStrength.Excellent)))
                {
                    _credentials = value;
                }
            }
        }
        private CancellationTokenSource _cancellationTokenSource;
        private int _taskTimeSeconds;
        public int CancelProcessAfterSeconds
        {
            get => _taskTimeSeconds;
            set
            {
                const int MIN_SECONDS = 1;
                const int MAX_SECONDS = 60;
                if (value >= MIN_SECONDS && value <= MAX_SECONDS)
                    _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(value));
            }
        }
        public Client() 
        {
            _ws = new ClientWebSocket();
            _connected = false;
            _serverUrl = string.Empty;
            _sessionKey = string.Empty;
            _credentials = default;
            const int DELAY_SECONDS = 30;
            _taskTimeSeconds = DELAY_SECONDS;
            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(DELAY_SECONDS));
        }

        public async Task ConnectToServer()
        {
            // Server URL is not set
            if (_serverUrl == string.Empty)
                return;

            // Connect to server
            await _ws.ConnectAsync(new Uri(_serverUrl), _cancellationTokenSource.Token);

            // Get response from the server
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result = await _ws.ReceiveAsync(buffer, _cancellationTokenSource.Token);

            // Get connection confirmation from the server
            var response = ServerMessage.Parser.ParseFrom(buffer, 0, result.Count);
            _connected = response.ServerMessageTypeCase.Equals(ServerMessageTypeOneofCase.Hello);
        }

        public async Task DisconnectFromServer()
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, _cancellationTokenSource.Token);
            _connected = false;
        }

        public async Task<RegistrationResponse> RegisterToServer(UserRegistration registration)
        {
            if (!registration.Valid)
                return RegistrationResponse.FormingRegisterRequestFailed;

            RegisterRequest? requestForm = registration.FormRegistrationRequest();

            // Failed to form register request protobuf
            if (requestForm is null)
                return RegistrationResponse.FormingRegisterRequestFailed;

            // Send user registration request to server
            ClientMessage clientMessage = new ClientMessage();
            clientMessage.RegisterRequest = requestForm;

            const bool END_OF_MESSAGE = true;
            byte[] buffer = clientMessage.ToByteArray();
            await _ws.SendAsync(buffer, WebSocketMessageType.Binary, END_OF_MESSAGE, _cancellationTokenSource.Token);

            // Get servers response
            const int OFFSET = 0;
            const int BYTECOUNT = 1024;
            buffer = new byte[BYTECOUNT];
            WebSocketReceiveResult response = await _ws.ReceiveAsync(buffer, _cancellationTokenSource.Token);
            ServerMessage serverMessage = ServerMessage.Parser.ParseFrom(buffer, OFFSET, response.Count);

            if (serverMessage.ServerMessageTypeCase != ServerMessageTypeOneofCase.RegisterResponse)
                return RegistrationResponse.RegisterRequestSendingFailed;

            RegisterResponse registerResponse = serverMessage.RegisterResponse;
            _sessionKey = (!string.IsNullOrEmpty(registerResponse.SessionKey)) ? registerResponse.SessionKey : _sessionKey;
            RegisterResultOneofCase cases = registerResponse.RegisterResultCase;

            switch (cases)
            {
                case RegisterResultOneofCase.None:
                    return RegistrationResponse.SessionKeyIssueFailed;
                case RegisterResultOneofCase.SessionKey:
                    return RegistrationResponse.SessionKeyIssueSuccessful;
                case RegisterResultOneofCase.DenyReason:
                    switch(registerResponse.DenyReason)
                    {
                        case Types.DenyReason.UsernameTaken:
                            return RegistrationResponse.UsernameIsTaken;
                        case Types.DenyReason.UsernameInvalidSymbols:
                            return RegistrationResponse.UsernameIsInvalid;
                        case Types.DenyReason.UsernameInvalidLength:
                            return RegistrationResponse.UsernameInvalidLength;
                        case Types.DenyReason.PasswordBlank:
                            return RegistrationResponse.PasswordIsBlank;
                        case Types.DenyReason.PasswordWeak:
                            return RegistrationResponse.PasswordIsWeak;
                        case Types.DenyReason.Unknown:
                            return RegistrationResponse.Unknown;
                        default:
                            return RegistrationResponse.Unknown;
                    }
                default:
                    return RegistrationResponse.Unknown;
            }
        }

        public async Task<AuthenticationResponse> LoginToServer()
        {
            if (string.IsNullOrEmpty(_credentials.Password) || string.IsNullOrEmpty(_credentials.Username))
                return AuthenticationResponse.ClientCredentialsNotValid;

            // Form request
            AuthRequest request = new AuthRequest();
            request.Username = _credentials.Username;
            request.Password = _credentials.Password;

            ClientMessage message = new ClientMessage();
            message.AuthRequest = request;

            // Send request to server
            const bool END_OF_MESSAGE = true;
            byte[] buffer = request.ToByteArray();
            await _ws.SendAsync(buffer, WebSocketMessageType.Binary, END_OF_MESSAGE, _cancellationTokenSource.Token);

            // Get servers response
            const int OFFSET = 0;
            const int BYTECOUNT = 1024;
            buffer = new byte[BYTECOUNT];
            WebSocketReceiveResult response = await _ws.ReceiveAsync(buffer, _cancellationTokenSource.Token);
            ServerMessage serverMessage = ServerMessage.Parser.ParseFrom(buffer, OFFSET, response.Count);

            // Process server message
            if (serverMessage.ServerMessageTypeCase != ServerMessageTypeOneofCase.AuthResponse)
                return AuthenticationResponse.AuthRequestReceiveFailed;

            if (serverMessage.AuthResponse.HasSessionKey)
            {
                _sessionKey = serverMessage.AuthResponse.SessionKey;
                return AuthenticationResponse.ClientAuthSuccess;
            }

            if (serverMessage.AuthResponse.HasDenyReason)
            {
                switch(serverMessage.AuthResponse.DenyReason)
                {
                    case AuthResponse.Types.DenyReason.UsernameInvalid:
                        return AuthenticationResponse.UserIsInvalidServerDeny;
                    case AuthResponse.Types.DenyReason.PasswordInvalid:
                        return AuthenticationResponse.PasswordIsInvalidServerDeny;
                    case AuthResponse.Types.DenyReason.Unknown:
                        return AuthenticationResponse.AuthFailedServerDeny;
                }
            }

            return AuthenticationResponse.ClientAuthFail;
        }

        public void Dispose()
        {
            // Free managed ClientWebSocket instance
            _ws?.Dispose();
        }
    }
}
