using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Pool
{
    public class ObjectPool
    {
        private Queue<GameObject> poolingQueue = new();
        public GameObject OriginalPrefab { get; private set; }
        public Transform Root { get; private set; }

        public ObjectPool(GameObject poolingPrefab, int initCount = 5)
        {
            OriginalPrefab = poolingPrefab;
            Root = new GameObject().transform;
            Root.name = $"{OriginalPrefab.name}_Root";

            Initialize(initCount);
        }

        private void Initialize(int count)
        {
            for (int i = 0; i < count; i++)
                poolingQueue.Enqueue(CreateNewObject());
        }

        private GameObject CreateNewObject()
        {
            GameObject newObject = Object.Instantiate(OriginalPrefab, Root);
            newObject.name = OriginalPrefab.name;
            newObject.gameObject.SetActive(false);
            return newObject;
        }

        public GameObject GetObject()
        {
            if (poolingQueue.Count > 0)
            {
                GameObject obj = poolingQueue.Dequeue();
                obj.transform.SetParent(Root.parent);
                obj.gameObject.SetActive(true);
                return obj;
            }

            GameObject newObj = CreateNewObject();
            newObj.transform.SetParent(Root.parent);
            newObj.gameObject.SetActive(true);
            return newObj;
        }

        public void ReturnObject(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(Root);
            poolingQueue.Enqueue(obj);
        }
    }
}
