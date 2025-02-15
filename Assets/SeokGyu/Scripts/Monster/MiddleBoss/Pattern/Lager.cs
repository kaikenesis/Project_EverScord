using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class Lager : MonoBehaviour
    {
        private float tickTime = 0.5f;
        private float countTime = 0.0f;
        private List<IStatus> hitList = new List<IStatus>();

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
                    hitList[i].TakeDamage(gameObject, 10.0f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                IStatus status;
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
                IStatus status;
                if (other.gameObject.TryGetComponent(out status))
                {
                    if(hitList.Contains(status))
                        hitList.Remove(status);
                }
            }
        }
    }
}
