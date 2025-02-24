using UnityEngine;

namespace EverScord.Pool
{
    public class PooledParticle : MonoBehaviour, IPoolable
    {
        private string assetGUID;

        public void OnParticleSystemStopped()
        {
            ResourceManager.Instance.ReturnToPool(gameObject, assetGUID);
        }

        public void Init(string id)
        {
            assetGUID = id;
        }

        public void SetGameObject(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
