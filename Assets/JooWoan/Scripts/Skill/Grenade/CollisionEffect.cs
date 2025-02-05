using UnityEngine;

namespace EverScord.Effects
{
    public class CollisionEffect : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private GameObject effect;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer != collisionLayer)
                return;

            Instantiate(effect);
        }
    }
}
