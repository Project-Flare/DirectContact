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

            var client = new Client();
            await client.ConnectToServer();
            Console.WriteLine($"Connection established = {client.IsConnected}");

            var userRegistration = new UserRegistration();
            userRegistration.Username = "pagarbiai_vacius_jusas_0";
            userRegistration.Password = ";IFlLyeOKBa|vJ.';vL56Z'$Ji'6&P";

            var registerResponse = await client.RegisterToServer(userRegistration);
            Console.WriteLine(registerResponse);

            Client.Credentials userCredentials = new Client.Credentials();
            userCredentials.Username = "pagarbiai_vacius_jusas_0";
            userCredentials.Password = ";IFlLyeOKBa|vJ.';vL56Z'$Ji'6&P";
            userCredentials.AuthToken = "ZSVMDyKeAAwbjHUsozdAuWM0Y+eCIwnKC8kqN45RL1oZUsoLJB0+WKIL/+PVJ2q/YYTwjDnE5hwJbFdmZ8RF88iWvezIUBm5DBrDMM8smWqKPIdudjLHITNpTt7By2iBhKldOH7CQHeLyzlRKSLtbrh2KYnF6bTlsBkj46BjqdY46JWdoEVgR+B0baFudvGVJVPQqdjHXA==|jDa4+TzcrMyPyqW0";
            client.UserCredentials = userCredentials;
            var loginResponse = await client.LoginToServer();
            Console.WriteLine(loginResponse);

            Console.WriteLine(client.UserCredentials.AuthToken);
            var authResponse = await client.TryAuthNewSession();
            Console.WriteLine(authResponse);
            Console.WriteLine(client.UserCredentials.AuthToken);

            await client.FillUserList();
            foreach (var user in client.UserList)
                Console.WriteLine(user);
        }
    }
}
