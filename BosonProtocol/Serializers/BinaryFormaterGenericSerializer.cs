using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace QuaternionProtocol.Serializers
{
    public class BinaryFormaterGenericSerializer<T> : IGenericSerializer<T,byte[]> where T: new()
    {
        
        public byte[] Serialize(T obj)
        {
            var biformater = new BinaryFormatter();
            using (var str = new MemoryStream())
            {
                biformater.Serialize(str, obj);
                return StreamHelper.GetBytes(str);
            }
        }

        public T Deserialize(byte[] stream)
        {
            var biformater = new BinaryFormatter();
            using (var str = new MemoryStream(stream))
            {
                return (T)biformater.Deserialize(str);
            }
        }

        public string Name => "BinaryFormater";

        public object Deserialize(Type targetType, byte[] rawData)
        {
            var biformater = new BinaryFormatter();
            using (var str = new MemoryStream(rawData))
            {
                return (T)biformater.Deserialize(str);
            }
        }
    }
}