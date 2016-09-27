namespace QuaternionProtocol.Serializers
{
    public interface IGenericSerializer<TSource, TDestTransportType> : ISerializer<TDestTransportType> where TSource : new()
    {
        TDestTransportType Serialize(TSource obj);
        TSource Deserialize(TDestTransportType stream);
    }
}