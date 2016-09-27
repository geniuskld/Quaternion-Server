using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ObjectsPool
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Pool<T> : ICollection<T> where T : class, new()
    {
        ConcurrentBag<T> _pool = new ConcurrentBag<T>();
        public Pool()
        {
        }

        public T Take()
        {
            T item;
            _pool.TryTake(out item);

            var poolSlotItem = item as IPoolSlot<T>;

            if (poolSlotItem != null)
            {
                poolSlotItem.PoolReference = this;
            }

            return item ?? (new T());
        }

        public void Release(object objectToRelease)
        {
            var typedObject = objectToRelease as T;

            if (typedObject == null) return;

            var poolSlot = typedObject as IPoolSlot<T>;

            if (poolSlot != null)
            {
                poolSlot.Clean();
            }
            else
            {
                var properties = typeof(T).GetRuntimeProperties().Where(x => x.CanWrite && x.CanRead && x.SetMethod != null);

                foreach (var property in properties)
                {

                    if (!property.PropertyType.IsByRef)
                    {
                        property.SetValue(typedObject, Activator.CreateInstance(property.PropertyType));
                    }
                    else
                    {
                        property.SetValue(typedObject, null);
                    }
                }
            }

            _pool.Add(typedObject);
        }


        #region ICollection
        public IEnumerator<T> GetEnumerator()
        {
            return _pool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _pool.Add(item);
        }

        public void Clear()
        {
            _pool = new ConcurrentBag<T>();
        }

        public bool Contains(T item)
        {
            return _pool.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _pool.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _pool.TryTake(out item);
        }

        public int Count => _pool.Count;
        public bool IsReadOnly => false;
        #endregion
    }
}
