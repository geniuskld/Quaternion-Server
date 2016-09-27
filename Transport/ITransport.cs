using System;
using Transport.Events;

namespace Transport
{
    public interface ITransport
    {
        event EventHandler<DataEventArgs<Exception>> OnError;
        string TransportName { get; }
        string Address { get; }

        void CloseConnection();
    }
}