using UnityEngine;

namespace EverScord.Effects
{
    public class CollisionEffect : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private GameObject effect;

        void OnCollisionEnter(Collision collision)
        {
            if ((1 << collision.gameObject.layer) != collisionLayer)
                return;

            var spawnedEffect = Instantiate(effect);
            spawnedEffect.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}
