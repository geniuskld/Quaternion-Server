using Transport.Connections;

namespace Transport.CommandsBase
{
    public interface IClientCommand
    {
        void Execute(ClientConnectionBase connection, IClientCommand entity);
    }
}