using System;
using UnityEngine;

namespace EverScord
{
    public class Lager : MonoBehaviour
    {
        public static Action OnEnter = delegate { };

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                OnEnter?.Invoke();
            }
        }
    }
}
