using System;
using System.IO;
using QuaternionProtocol.Serializers;
using System.Xml;
namespace Tests.Emulators.Transport
{
    public class ProtoBufGenericSerializer<T> : IGenericSerializer<T, byte[]> where T : new()
    {
        public byte[] Serialize(T obj)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize(byte[] stream)
        {
            using (var ms = new MemoryStream(stream))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        public string Name => "ProtoBuf";

        public object Deserialize(Type targetType, byte[] rawData)
        {
            return Deserialize(rawData);
        }
    }
}