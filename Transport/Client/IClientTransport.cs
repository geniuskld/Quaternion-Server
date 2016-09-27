using System;
using System.IO;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Events;

namespace Transport.Client
{
    public interface IClientTransport: ITransport
    {
        OperationState SendData<T>(ProtocolBase<T> data);
        OperationState SendData(Stream data);
        OperationState Connect(string endpointAddress);
        OperationState RegisterCommand<T>(string commandName, T executeAction) where T : IClientCommand;
    }
}