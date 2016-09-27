using System;
using System.IO;
using BosonProtocol;
using BosonProtocol.Protocol;
using Transport;
using Transport.Events;

namespace SignalRServer
{
    public class SignalRTransport: TransportBase<string>
    {
        protected override void Ping(GenericEventArgs<object> obj)
        {
            throw new NotImplementedException();
        }

        public override string TransportName => "SignalRTransport";
        public override string ServerAddress { get; }
        public override void DisconnectClient(string connectionId)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override string GetClientAddressInfo(string connectionId)
        {
            throw new NotImplementedException();
        }

        public override OperationState SendData<T>(string connectionId, ProtocolBase<T> data)
        {
            var protoBytes = data as ProtocolBase<string>;

            if (protoBytes != null)
            {
                
               return new OperationState {IsSuccessful = true};
            }

            return new OperationState { IsSuccessful = false, Details = "ProtocolBase<string> is correct type of Proto" };
        }

        public override OperationState SendData(Stream data, string connectionId)
        {
            throw new NotImplementedException();
        }

        public override DateTime LastPingTimeFor(string connectionId)
        {
            throw new NotImplementedException();
        }

        public override TransportStats GetStats(string connectionId)
        {
            throw new NotImplementedException();
        }

        public override void ChangeConnectionId(string oldConnectionId, string newConnectionId)
        {
            throw new NotImplementedException();
        }
    }
}
