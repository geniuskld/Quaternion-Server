using System;
using QuaternionProtocol.Protocol;
using Transport.Connections;

namespace Transport.Events
{
    public class ServerGenericEventArgs<T>: DataEventArgs<T>
    {
        public ServerConnectionBase ServerConnectionBase;
    }
}