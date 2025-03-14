using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using UnityEngine.AddressableAssets;

namespace EverScord.Pool
{
    public class PoolableVfx : MonoBehaviour, IPoolable
    {
        [SerializeField] private float returnTime;
        [SerializeField] private AssetReference reference;
        private VisualEffect vfx;

        void Awake()
        {
            vfx = GetComponent<VisualEffect>();
        }

        public void SetDuration(float time)
        {
            returnTime = time;
        }

        public void SetGameObject(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Play(float delay)
        {
            Invoke(nameof(Play), delay);
        }

        public void Play()
        {
            if (!vfx)
                return;

            vfx.Play();
            StartCoroutine(ReturnVfx());
        }

        public void Stop()
        {
            if (!vfx)
                return;

            vfx.Stop();
        }

        public IEnumerator ReturnVfx()
        {
            yield return new WaitForSeconds(returnTime);
            ResourceManager.Instance.ReturnToPool(this, reference.AssetGUID);
        } 
    }
}
