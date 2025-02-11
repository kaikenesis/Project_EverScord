using UnityEngine;
using EverScord.Weapons;

namespace EverScord.Pool
{
    public class BulletPool : ObjectPool<Bullet>
    {
        private GameObject tracerPrefab;
        public Transform Root { get; private set; }

        public BulletPool(GameObject tracerPrefab, Transform rootParent, int count = 5) : base(0)
        {
            Root = new GameObject().transform;
            Root.name = $"{tracerPrefab.name}_Root";
            Root.parent = rootParent;

            this.tracerPrefab = tracerPrefab;

            for (int i = 0; i < count; i++)
                poolingQueue.Enqueue(CreateObject());
        }

        public override Bullet CreateObject()
        {
            Bullet bullet = new Bullet();
            TrailRenderer trailRenderer = Object.Instantiate(tracerPrefab, Root).GetComponent<TrailRenderer>();

            bullet.SetTracerEffect(trailRenderer);
            bullet.TracerEffect.gameObject.SetActive(false);
            
            return bullet;
        }

        public override Bullet GetObject()
        {
            Bullet bullet = poolingQueue.Count > 0 ? poolingQueue.Dequeue() : CreateObject();

            bullet.TracerEffect.transform.SetParent(Root.parent);
            bullet.TracerEffect.gameObject.SetActive(true);
            return bullet;
        }

        public override void ReturnObject(Bullet bullet)
        {
            bullet.TracerEffect.gameObject.SetActive(false);
            bullet.TracerEffect.transform.SetParent(Root);
            poolingQueue.Enqueue(bullet);
        }
    }

    public enum TracerType
    {
        EFFECT_TRACER_NED,
        EFFECT_TRACER_UNI,
        EFFECT_TRACER_US
    }
}
