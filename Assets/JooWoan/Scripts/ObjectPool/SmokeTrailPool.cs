using UnityEngine;

namespace EverScord.Pool
{
    public class SmokeTrailPool : ObjectPool<SmokeTrail>
    {
        public GameObject OriginalPrefab { get; private set; }
        public Transform Root { get; private set; }

        public SmokeTrailPool(GameObject smokePrefab, Transform rootParent, int count = 5) : base(0)
        {
            OriginalPrefab = smokePrefab;
            Root = new GameObject().transform;
            Root.name = $"{smokePrefab.name}_Root";
            Root.parent = rootParent;

            for (int i = 0; i < count; i++)
                poolingQueue.Enqueue(CreateObject());
        }

        public override SmokeTrail CreateObject()
        {
            GameObject smoke = Object.Instantiate(OriginalPrefab, Root);
            smoke.SetActive(false);
            return smoke.GetComponent<SmokeTrail>();
        }

        public override SmokeTrail GetObject()
        {
            SmokeTrail smokeTrail = poolingQueue.Count > 0 ? poolingQueue.Dequeue() : CreateObject();

            smokeTrail.transform.SetParent(Root.parent);
            smokeTrail.gameObject.SetActive(true);

            return smokeTrail;
        }

        public override void ReturnObject(SmokeTrail smokeTrail)
        {
            smokeTrail.gameObject.SetActive(false);
            smokeTrail.transform.SetParent(Root);
            poolingQueue.Enqueue(smokeTrail);
        }
    }
}
