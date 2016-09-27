using Transport.Connections;

namespace Transport.Events
{
    public class ClientGenericEventArgs<T>: DataEventArgs<T>
    {
        public ClientConnectionBase ConnectionBase;
    }
}