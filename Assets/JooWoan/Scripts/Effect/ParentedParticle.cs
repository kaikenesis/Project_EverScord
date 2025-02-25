using UnityEngine;

namespace EverScord.Effects
{
    public class ParentedParticle : MonoBehaviour
    {
        Transform particleTransform;

        void Awake()
        {
            particleTransform = transform;
        }

        void Update()
        {
            particleTransform.rotation = Quaternion.identity;
        }
    }
}
