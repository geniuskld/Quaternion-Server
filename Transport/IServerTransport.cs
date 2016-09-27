using QuaternionProtocol;
using Transport.CommandsBase;

namespace Transport
{
    public interface IServerTransport:ITransport
    {
        OperationState RegisterCommand<T>(string commandName, T executeAction) where T : IServerCommand;
    }
}