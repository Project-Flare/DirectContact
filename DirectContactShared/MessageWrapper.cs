using DirectContactShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactServer
{
    public interface MessageWrapper
    {
        public MessageType type { get; }
    }
}
