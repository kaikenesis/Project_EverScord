using UnityEngine;

namespace EverScord
{
    public class DamageTest : MonoBehaviour, IEnemy
    {
        public void TestDamage(GameObject sender, float value)
        {
            Debug.Log($"{name} || Sender : {sender.name}, Damage : {value}");
        }

        public void DecreaseHP(float hp)
        {
            throw new System.NotImplementedException();
        }

        public void StunMonster(float stunTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
