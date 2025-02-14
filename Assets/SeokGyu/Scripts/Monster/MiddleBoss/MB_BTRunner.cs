using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class MB_BTRunner : BTRunner
    {
        [SerializeField] private MiddleBossData data;
        [SerializeField] private MB_Controller owner;

        protected override void Init()
        {
            blackboard.SetValue("IsDead", false);
            blackboard.SetValue("IsCooldown", false);
            blackboard.SetValue("Phase", 0);
            blackboard.SetValue("Owner", owner);
        }

        //public void IncreaseHealth(float value)
        //{
        //    curHealth += value;

        //    if (curHealth >= maxHealth)
        //        curHealth = maxHealth;
        //}

        //public void DecreaseHealth(float value)
        //{
        //    curHealth -= value;

        //    if (curHealth <= 0.0f)
        //        curHealth = 0.0f;
        //}
    }
}
