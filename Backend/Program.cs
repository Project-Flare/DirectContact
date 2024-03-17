using System.Net.WebSockets;
using Google.Protobuf;
using static Flare.RegisterResponse;
using static Flare.ServerMessage;
using Flare;
 
namespace Backend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            /*using (var ws = new ClientWebSocket())
            {
                byte[] buffer = new byte[1024];
                await ws.ConnectAsync(new Uri("wss://ws.project-flare.net/"), CancellationToken.None);
                var serverResponse = await ws.ReceiveAsync(buffer, CancellationToken.None);


               *//* RegisterRequest registrationRequest = new RegisterRequest();
                registrationRequest.Username = "skibidi_toletai_24563";
                registrationRequest.Password = "q}TeHBu(8y=jybKo_]-1eJqj=v1ZjTK";
                ClientMessage clientMessage = new ClientMessage();
                clientMessage.RegisterRequest = registrationRequest;*//*
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.Username = "airidas_vengrauskas_2004";
                loginRequest.Password = "n6Ct4C{!\\\"H9--_{[{A\"/w2RkQz?8`i4}@";
                ClientMessage clientMessage = new ClientMessage();
                clientMessage.LoginRequest = loginRequest;
                buffer = clientMessage.ToByteArray();
                await ws.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
                buffer = new byte[1024];
                Console.WriteLine("Sending the request");
                serverResponse = await ws.ReceiveAsync(buffer, CancellationToken.None);
                Console.WriteLine("Request received");
                var serverMessage = ServerMessage.Parser.ParseFrom(buffer, 0, serverResponse.Count);

            }*/

            var client = new Client();
            await client.ConnectToServer();
            Console.WriteLine($"Connection established = {client.IsConnected}");

            var userRegistration = new UserRegistration();
            userRegistration.Username = "pagarbiai_vacius_jusas_0";
            userRegistration.Password = ";IFlLyeOKBa|vJ.';vL56Z'$Ji'6&P";

            var response = await client.RegisterToServer(userRegistration);
            Console.WriteLine(response);
        }
    }
}
