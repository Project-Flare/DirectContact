using Flare;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Flare.RegisterResponse;
using static Flare.ServerMessage;

namespace Backend
{
    public enum ServerRegisterResponse
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


        public Client() 
        {
            _ws = new ClientWebSocket();
            _connected = false;
            _serverUrl = string.Empty;
        }

        public async Task ConnectToServer()
        {
            // Server URL is not set
            if (_serverUrl == string.Empty)
                return;

            // Connect to server
            await _ws.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);

            // Get response from the server
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result = await _ws.ReceiveAsync(buffer, CancellationToken.None);

            // Get connection confirmation from the server
            var response = ServerMessage.Parser.ParseFrom(buffer, 0, result.Count);
            _connected = response.ServerMessageTypeCase.Equals(ServerMessageTypeOneofCase.Hello);
        }

        public async Task DisconnectFromServer()
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            _connected = false;
        }

        public async Task<ServerRegisterResponse> RegisterToServer(UserRegistration registration)
        {
            if (registration is null)
                return ServerRegisterResponse.FormingRegisterRequestFailed;

            if (!registration.Valid)
                return ServerRegisterResponse.FormingRegisterRequestFailed;

            RegisterRequest? requestForm = registration.FormRegistrationRequest();

            // Failed to form register request protobuf
            if (requestForm is null)
                return ServerRegisterResponse.FormingRegisterRequestFailed;

            // Send user registration request to server
            ClientMessage clientMessage = new ClientMessage();
            clientMessage.RegisterRequest = requestForm;
            byte[] buffer = clientMessage.ToByteArray();
            await _ws.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);

            // Get servers response
            const int Offset = 0;
            const int ByteCount = 1024;
            buffer = new byte[ByteCount];
            WebSocketReceiveResult response = await _ws.ReceiveAsync(buffer, CancellationToken.None);
            ServerMessage serverMessage = ServerMessage.Parser.ParseFrom(buffer, Offset, response.Count);

            if (serverMessage.ServerMessageTypeCase != ServerMessageTypeOneofCase.RegisterResponse)
                return ServerRegisterResponse.RegisterRequestSendingFailed;

            RegisterResponse registerResponse = serverMessage.RegisterResponse;
            RegisterResultOneofCase cases = registerResponse.RegisterResultCase;

            switch (cases)
            {
                case RegisterResultOneofCase.None:
                    return ServerRegisterResponse.SessionKeyIssueFailed;
                case RegisterResultOneofCase.SessionKey:
                    return ServerRegisterResponse.SessionKeyIssueSuccessful;
                case RegisterResultOneofCase.DenyReason:
                    switch(registerResponse.DenyReason)
                    {
                        case Types.DenyReason.UsernameTaken:
                            return ServerRegisterResponse.UsernameIsTaken;
                        case Types.DenyReason.UsernameInvalidSymbols:
                            return ServerRegisterResponse.UsernameIsInvalid;
                        case Types.DenyReason.UsernameInvalidLength:
                            return ServerRegisterResponse.UsernameInvalidLength;
                        case Types.DenyReason.PasswordBlank:
                            return ServerRegisterResponse.PasswordIsBlank;
                        case Types.DenyReason.PasswordWeak:
                            return ServerRegisterResponse.PasswordIsWeak;
                        case Types.DenyReason.Unknown:
                            return ServerRegisterResponse.Unknown;
                        default:
                            return ServerRegisterResponse.Unknown;
                    }
                default:
                    return ServerRegisterResponse.Unknown;
            }
        }

        public void Dispose()
        {
            // Free managed ClientWebSocket instance
            _ws?.Dispose();
        }
    }
}
