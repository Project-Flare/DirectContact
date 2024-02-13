using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DirectContactServer;

namespace DirectContactShared.Messages
{
    internal class ServerIntroduce : MessageWrapper
    {
        public override MessageType type => MessageType.ServerIntroduce;

        public string serverName;
        public string sessionKey;

        public ServerIntroduce(string serverName, string sessionKey)
        {
            this.serverName = serverName;
            this.sessionKey = sessionKey;
        }
    }
}
