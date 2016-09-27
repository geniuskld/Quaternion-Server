using System.Security.Cryptography;
using QuaternionProtocol.Serializers;

namespace QuaternionProtocol.Protocol
{

    public abstract class ProtocolBase<TDestTrasportableType>
    {
        public string ProtocolName => GetType().Name;
        public TDestTrasportableType Data { get; protected set; }
        public abstract TDestTrasportableType GetDataToSend();
        public abstract TDestTrasportableType CleanData(TDestTrasportableType receivedData);
        public abstract TDestTrasportableType GetBody();
    }
    
}