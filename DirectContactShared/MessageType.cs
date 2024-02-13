using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactShared
{
    /// <summary>
    /// Represents the set of possible message types in bidirectional communication
    /// </summary>
    public enum MessageType
    {
        ClientCallout,
        ClientSubmitIdentity,

        ServerIntroduce,
        ServerAcknowledgeIdentity,
    }
}
