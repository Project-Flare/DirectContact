using DirectContactServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DirectContactShared.Messages
{
    internal class ClientSubmitIdentity : MessageWrapper
    {
        public MessageType type => MessageType.ClientSubmitIdentity;

        public string userName;
        public ECDiffieHellmanPublicKey publicKey;

        public ClientSubmitIdentity(string userName, ECDiffieHellmanPublicKey publicKey)
        {
            this.userName = userName;
            this.publicKey = publicKey;
        }
    }
}
