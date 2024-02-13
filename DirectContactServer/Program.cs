using Fleck;

namespace DirectContactServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new WebSocketServer("ws://0.0.0.0:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () => Console.WriteLine("Open!");
                socket.OnClose = () => Console.WriteLine("Close!");
                socket.OnMessage = message => socket.Send(message);
                socket.OnBinary = message => Server.OnMessage(message);
            });
        }
    }
}
