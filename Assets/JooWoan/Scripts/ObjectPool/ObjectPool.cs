using System.Collections.Generic;

namespace EverScord.Pool
{
    public class ObjectPool<T> where T : class, new()
    {
        protected Queue<T> poolingQueue = new Queue<T>();
        public ObjectPool(int count = 5)
        {
            for (int i = 0; i < count; i++)
                poolingQueue.Enqueue(CreateObject());
        }

        public virtual T CreateObject()
        {
            return new T();
        }

        public virtual T GetObject()
        {
            if (poolingQueue.Count > 0)
                return poolingQueue.Dequeue();

            return CreateObject();
        }

        public virtual void ReturnObject(T obj)
        {
            poolingQueue.Enqueue(obj);
        }
    }
}
