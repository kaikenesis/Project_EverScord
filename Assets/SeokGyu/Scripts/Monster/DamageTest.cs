using EverScord.Character;
using EverScord.Effects;
using UnityEngine;

namespace EverScord
{
    public class DamageTest : MonoBehaviour, IEnemy
    {
        public void TestDamage(GameObject sender, float value)
        {
            //Debug.Log($"{name} || Sender : {sender.name}, Damage : {value}");
        }

        public void DecreaseHP(float hp, CharacterControl attacker)
        {
            throw new System.NotImplementedException();
        }

        public void StunMonster(float stunTime)
        {
            throw new System.NotImplementedException();
        }

        public BlinkEffect GetBlinkEffect()
        {
            throw new System.NotImplementedException();
        }

        public float GetDefense()
        {
            throw new System.NotImplementedException();
        }
    }
}
