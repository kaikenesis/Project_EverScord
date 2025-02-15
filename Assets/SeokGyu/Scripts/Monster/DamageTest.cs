using UnityEngine;

namespace EverScord
{
    public class DamageTest : MonoBehaviour, IStatus
    {
        public void TakeDamage(GameObject sender, float damage)
        {
            Debug.Log($"{name} || Sender : {sender.name}, Damage : {damage}");
        }
    }
}
