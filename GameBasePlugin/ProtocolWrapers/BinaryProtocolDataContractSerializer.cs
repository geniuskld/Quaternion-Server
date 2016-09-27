using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using QuaternionProtocol.Serializers;

namespace GameBasePlugin.ProtocolWrapers
{
    public static class BinaryProcolHelper<T> where T : new()
    {
        public static ProtocolBase<byte[]> GetProtocol(T data)
        {
           return BinaryProtocol.FromData(data, GetSerializer());
        } 
        public static IGenericSerializer<T, byte[]> GetSerializer()
        {
            return new BinaryFormaterGenericSerializer<T>();
        } 
    }
}