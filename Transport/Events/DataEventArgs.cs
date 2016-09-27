using System;
using QuaternionProtocol.Protocol;

namespace Transport.Events
{
    public class DataEventArgs<T>: EventArgs
    {
        public T DataReceived;
        public ProtocolBase<T> Protocol;
    }
}