using UnityEngine;

namespace EverScord.Pool
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public enum PoolType
        {
            EFFECT_TRACER_NED,
            EFFECT_TRACER_UNI,
            EFFECT_TRACER_US,
        }

        public GameObject OriginalPrefab { get; private set; }
        public Transform Root { get; private set; }

        public GameObjectPool(GameObject poolingPrefab, int count = 5)
        {
            OriginalPrefab = poolingPrefab;
            Root = new GameObject().transform;
            Root.name = $"{OriginalPrefab.name}_Root";

            for (int i = 0; i < count; i++)
                poolingQueue.Enqueue(CreateObject());
        }

        public override GameObject CreateObject()
        {
            GameObject newObject = Object.Instantiate(OriginalPrefab, Root);
            newObject.name = OriginalPrefab.name;
            newObject.gameObject.SetActive(false);
            return newObject;
        }

        public override GameObject GetObject()
        {
            if (poolingQueue.Count > 0)
            {
                GameObject obj = poolingQueue.Dequeue();
                obj.transform.SetParent(Root.parent);
                obj.gameObject.SetActive(true);
                return obj;
            }

            GameObject newObj = CreateObject();
            newObj.transform.SetParent(Root.parent);
            newObj.gameObject.SetActive(true);
            return newObj;
        }

        public override void ReturnObject(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(Root);
            poolingQueue.Enqueue(obj);
        }
    }
}
