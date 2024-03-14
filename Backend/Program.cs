using System.Net.Sockets;
using System.Net.WebSockets;
using Flare;
using Google.Protobuf;
using static Flare.RegisterResponse;
using static Flare.ServerMessage;
namespace Backend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string ServerUrl = "wss://ws.project-flare.net/";
            const int MaxServerResponseTimeSeconds = 3000;

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
                    ServerMessage response = ServerMessage.Parser.ParseFrom(buffer, 0, serverResponse.Count);

                    if (response.ServerMessageTypeCase != ServerMessageTypeOneofCase.Hello)
                    {
                        Console.WriteLine("Failed to greet the server");
                        return;
                    }
                    Console.WriteLine("Connected to the server successfully");

                    // Attempt user registration
                    Registration userRegistration = new Registration();

                    /*// Max 10 times for setting username
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
                    }*/

                    /*userRegistration.TrySetPassword("=B>t<|&a9<\\{+Vbj^B<#k(_~$G-*XMJ");
                    userRegistration.TrySetUsername("manfredas_lamsargis_2003");*/
                    userRegistration.TrySetPassword("v6SeNVRZw_OX8T-ye5~0/l=03^lX*CW");
                    userRegistration.TrySetUsername("manfredas_lamsargis_2021");
                    RegisterRequest? request = userRegistration.FormRegistrationRequest();
                    if (request is null)
                    {
                        Console.WriteLine("Forming registration request for server failed.");
                    }
                    else
                    {
                        ClientMessage mesg = new ClientMessage();
                        mesg.RegisterRequest = request;

                        // Send request for user registration
                        byte[] bytes = mesg.ToByteArray();
                        Console.WriteLine(webSocket.State);
                        cts.CancelAfter(TimeSpan.FromSeconds(80));
                        await webSocket.SendAsync(bytes, WebSocketMessageType.Binary, true, cts.Token);
                        bytes = new byte[1024];
                        WebSocketReceiveResult registerResponse = await webSocket.ReceiveAsync(bytes, cts.Token);
                        ServerMessage regResponse = ServerMessage.Parser.ParseFrom(bytes, 0, registerResponse.Count);
                        if (regResponse.ServerMessageTypeCase == ServerMessageTypeOneofCase.RegisterResponse)
                        {
                            RegisterResponse registrationResponse = new RegisterResponse(regResponse.RegisterResponse);
                            //RegisterResponse.Types.DenyReason DenyReason
                            RegisterResponse.RegisterResultOneofCase reasons = registrationResponse.RegisterResultCase;
                            switch(reasons)
                            {
                                case RegisterResultOneofCase.None:
                                    Console.WriteLine("Failed to receive session key");
                                    break;
                                case RegisterResultOneofCase.DenyReason:
                                    Console.WriteLine("Registration was denied because " + registrationResponse.DenyReason);
                                    break;
                                case RegisterResultOneofCase.SessionKey:
                                    Console.WriteLine("Given session key is: " + registrationResponse.SessionKey);
                                    break;
                            }
                        }
                    }
                    Console.Write(webSocket.State);

                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
