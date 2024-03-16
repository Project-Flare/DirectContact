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

                // Specify the longest possible time you want to wait for the service
                // So if the response from the server is longer than 20 seconds, System.Threading.Tasks.TaskCanceledException is thrown
                client.CancelProcessAfterSeconds = 20;

                await client.ConnectToServer();

                if (!client.ConnectedToServer)
                {
                    Console.WriteLine("Failed to connect to server...");
                    return;
                }

                // User credentials
                const string USERNAME = "airidas_vengrauskas_2008";
                const string PASSWORD = "n6Ct4C{!\\\"H9--_{[{A\"/w2RkQz?8`i4}@";


                //////////////////////// LOG IN TO SERVER
                /*client.Credentials = new ClientCredentials(USERNAME, PASSWORD);
                Console.WriteLine(client.Credentials.ToString());

                AuthenticationResponse authResponse = await client.LoginToServer();

                if (authResponse.Equals(AuthenticationResponse.ClientAuthSuccess))
                    Console.WriteLine("Login to clients account is successful: " + client.SessionKey);
                else
                    Console.WriteLine("Login to clients account failed because: " + authResponse);*/

                //////////////////////// REGISTER TO SERVER

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

                RegistrationResponse serverResponse = await client.RegisterToServer(registration);

                if (serverResponse.Equals(RegistrationResponse.SessionKeyIssueSuccessful))
                {
                    Console.WriteLine("User registration is successful");
                }
                else if (serverResponse.Equals(RegistrationResponse.SessionKeyIssueFailed))
                {
                    Console.WriteLine("Session key issue failed");
                }
                else
                {
                    Console.WriteLine("Failed to register new user because " + serverResponse);
                }

                await client.DisconnectFromServer();

                if (!client.ConnectedToServer)
                {
                    Console.WriteLine("Session is over");
                }
            }
        }
    }
}
