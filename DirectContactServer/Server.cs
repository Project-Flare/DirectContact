using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactServer
{
    internal static class Server
    {
        internal static void OnMessage(byte[] message)
        {
            var deserializer = new DataContractSerializer(typeof(MessageWrapper));
        }
    }
}
