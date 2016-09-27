using Transport.Connections;

namespace Transport.CommandsBase
{
    public interface IServerCommand
    {
        void Execute<T>(ServerConnection<T> connection, IServerCommand entity);
    }
}