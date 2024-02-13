using DirectContactShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactServer
{
    public abstract class MessageWrapper
    {
        public abstract MessageType type { get; }
    }
}
