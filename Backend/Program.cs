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

            using (var client = new Client())
            {
                // Specify server url
                client.ServerUrl = "wss://ws.project-flare.net/";

                await client.ConnectToServer();

                if (!client.ConnectedToServer)
                {
                    Console.WriteLine("Failed to connect to server...");
                    return;
                }

                // Form new user registration
                const string USERNAME = "jelabanu_ostakorov84_99";
                const string PASSWORD = "n6Ct4C{!\\\"H--_{[{A\"/w2RkQz?8`i4}@";

                UserRegistration registration = new UserRegistration();

                registration.Username = USERNAME;

                if (!registration.UsernameValid)
                {
                    Console.Write("Username is not valid because "
                        + UserRegistration.ValidifyUsername(USERNAME));
                    return;
                }

                registration.Password = PASSWORD;

                if (!registration.PasswordValid)
                {
                    Console.WriteLine("Password is not valid because password is: "
                        + UserRegistration.EvaluatePassword(PASSWORD));
                    return;
                }

                if (!registration.Valid)
                {
                    Console.WriteLine("Filling registration form failed...");
                    return;
                }

                ServerRegisterResponse serverResponse = await client.RegisterToServer(registration);

                if (serverResponse.Equals(ServerRegisterResponse.SessionKeyIssueSuccessful))
                {
                    Console.WriteLine("User registration is successful");
                }
                else if (serverResponse.Equals(ServerRegisterResponse.SessionKeyIssueFailed))
                {
                    Console.WriteLine("Session key issue failed");
                }
                else
                {
                    Console.WriteLine("Failed to register new user because " + serverResponse);
                }

                // TODO: not implemented normally
                await client.DisconnectFromServer();

                if (!client.ConnectedToServer)
                {
                    Console.WriteLine("Session is over");
                }
            }
        }
    }
}
