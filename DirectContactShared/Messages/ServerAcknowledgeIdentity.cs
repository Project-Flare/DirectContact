using DirectContactServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactShared.Messages
{
    internal class ServerAcknowledgeIdentity : MessageWrapper
    {
        public override MessageType type => MessageType.ServerAcknowledgeIdentity;
    }
}
