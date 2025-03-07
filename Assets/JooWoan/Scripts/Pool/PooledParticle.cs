using UnityEngine;

namespace EverScord.Pool
{
    public class PooledParticle : MonoBehaviour, IPoolable
    {
        [SerializeField] ParticleSystem effect;
        private string assetGUID;

        public void OnParticleSystemStopped()
        {
            ResourceManager.Instance.ReturnToPool(this, assetGUID);
        }

        public void Init(string id)
        {
            assetGUID = id;
        }

        public void SetGameObject(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Emit(int amount = 1)
        {
            effect.Emit(amount);
        }

        public void Play()
        {
            effect.Play();
        }
    }
}
