namespace ObjectsPool
{
    public  interface IPoolSlot<T> where T: class, new()
    {
        Pool<T> PoolReference { get; set; }
        void Clean();

        void Release();
    }
}