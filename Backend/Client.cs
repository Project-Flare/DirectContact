using Flare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static Flare.ServerMessage;

namespace Backend
{
    public enum ServerRegisterResponse
    {
        UsernameIsTaken,
        UsernameIsInvalid,
        PasswordIsInvalid,
        Unknown
    }

    // use new client object withing using brackets
    public class Client : IDisposable
    {
        private ClientWebSocket _ws;
        private bool _connected;
        public bool IsConnected 
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
            // Sends close frame
            //await _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            //_connected = _ws.State == WebSocketState.Closed;
            throw new NotImplementedException();
        }

        public ServerRegisterResponse RegisterToServer(Registration registration)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // Free managed ClientWebSocket instance
            _ws?.Dispose();
        }
    }
}
