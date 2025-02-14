using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord.Weapons
{
    [CreateAssetMenu(fileName = "Bullet Tracer Info", menuName = "EverScord/Bullet Tracer Info")]
    public class BulletInfo : ScriptableObject
    {
        [field: SerializeField] public Material TracerMaterial { get; private set; }
        [field: SerializeField] public Gradient TracerGradient { get; private set; }
    }
}
