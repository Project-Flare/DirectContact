using System.Net.WebSockets;
using Flare;
using Google.Protobuf;
using static Flare.ServerMessage;
namespace Backend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string ServerUrl = "wss://ws.project-flare.net/";
            const int MaxServerResponseTimeSeconds = 5;

            var serverUri = new Uri(ServerUrl);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(MaxServerResponseTimeSeconds));

            using (var webSocket = new ClientWebSocket())
            {
                try
                {
                    // Wait for connection to the server
                    await webSocket.ConnectAsync(serverUri, cts.Token);

                    // Get "Hello" message from the server
                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult serverResponse = await webSocket.ReceiveAsync(buffer, cts.Token);
                    ServerMessage response = Parser.ParseFrom(buffer, 0, serverResponse.Count);

                    if (response.ServerMessageTypeCase != ServerMessageTypeOneofCase.Hello)
                    {
                        Console.WriteLine("Failed to greet the server");
                        return;
                    }
                    Console.WriteLine("Connected to the server successfully");

                    // Attempt user registration
                    Registration userRegistration = new Registration();
                    
                    // Max 10 times for setting username
                    for (int times = 0; times < 10; times++)
                    {
                        Console.Write("Username: ");
                        string? username = Console.ReadLine();
                        if (username is null)
                            return;
                        bool usernameValid = userRegistration.UsernameValid(username);
                        Console.WriteLine("Username is valid: " + userRegistration.TrySetUsername(username));
                        if (usernameValid)
                        {
                            Console.WriteLine("Welcome, " + userRegistration.Username);
                            break;
                        }
                    }

                    // Max 20 tries to set correctly account password
                    for (int times = 0; times < 10; times++)
                    {
                        Console.Write("Password: ");
                        string? password = Console.ReadLine();
                        if (password is null)
                            return;
                        bool passwordValid = userRegistration.TrySetPassword(password);
                        if (passwordValid)
                        {
                            Console.WriteLine("Password: " + password + '\n' + userRegistration.EvaluatePassword(password));
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Password objected: " + password + '\n' + userRegistration.EvaluatePassword(password));
                        }
                    }

                    RegisterRequest? request = userRegistration.FormRegistrationRequest();
                    if (request is null)
                    {
                        Console.WriteLine("Forming registration request for server failed.");
                    }
                    else
                    {
                        // Send request for user registration
                        byte[] bytes = request.ToByteArray();
                        await webSocket.SendAsync(bytes, WebSocketMessageType.Binary, true, cts.Token);

                        WebSocketReceiveResult registerResponse = await webSocket.ReceiveAsync(bytes, cts.Token);
                        ServerMessage regResponse = Parser.ParseFrom(bytes, 0, serverResponse.Count);
                        if (!response.ServerMessageTypeCase.Equals(ServerMessageTypeOneofCase.RegisterResponse))
                        {
                            Console.WriteLine("Registration failed...");
                        }
                        else
                        {
                            Console.WriteLine(userRegistration.Username + " your account is registered to Direct Contact!");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Service terminated because of:\n" + ex.Message);
                }
            }
        }
    }
}
