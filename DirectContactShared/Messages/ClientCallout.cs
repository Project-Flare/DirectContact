using DirectContactServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactShared
{
    // 
    public class ClientCallout : MessageWrapper
    {
        public override MessageType type => MessageType.ClientCallout;
    }
}
