using System;

namespace QuaternionProtocol.Serializers
{
    public interface ISerializer<in TDestTransportType>
    {
        string Name { get; }
        object Deserialize(Type targetType, TDestTransportType rawData);
    }
}