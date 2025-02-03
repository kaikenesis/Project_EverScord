using UnityEngine;

namespace EverScord.Pool
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObject OriginalPrefab { get; private set; }
        public Transform Root { get; private set; }

        public GameObjectPool(GameObject poolingPrefab, Transform rootParent, int count = 5) : base(0)
        {
            OriginalPrefab = poolingPrefab;
            Root = new GameObject().transform;
            Root.name = $"{OriginalPrefab.name}_Root";
            Root.parent = rootParent;

            for (int i = 0; i < count; i++)
                poolingQueue.Enqueue(CreateObject());
        }

        public override GameObject CreateObject()
        {
            GameObject newObject = Object.Instantiate(OriginalPrefab, Root);
            newObject.gameObject.SetActive(false);
            return newObject;
        }

        public override GameObject GetObject()
        {
            GameObject newObj = poolingQueue.Count > 0 ? poolingQueue.Dequeue() : CreateObject();

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
