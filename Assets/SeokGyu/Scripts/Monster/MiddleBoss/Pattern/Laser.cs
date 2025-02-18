using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class Laser : MonoBehaviour
    {
        private float tickTime = 0.5f;
        private float countTime = 0.0f;
        private List<IEnemy> hitList = new List<IEnemy>();

        private void Update()
        {
            if(countTime < tickTime)
            {
                countTime += Time.deltaTime;
                
            }
            else
            {
                countTime = 0;
                for (int i = 0; i < hitList.Count; i++)
                {
                    hitList[i].TestDamage(gameObject, 10.0f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                IEnemy status;
                if(other.gameObject.TryGetComponent(out status))
                {
                    hitList.Add(status);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                IEnemy status;
                if (other.gameObject.TryGetComponent(out status))
                {
                    if(hitList.Contains(status))
                        hitList.Remove(status);
                }
            }
        }
    }
}
