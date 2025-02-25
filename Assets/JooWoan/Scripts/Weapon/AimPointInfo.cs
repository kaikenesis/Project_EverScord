using UnityEngine;
using Photon.Pun;

namespace EverScord.Weapons
{
    public class AimPointInfo : MonoBehaviour
    {
        private PhotonView photonView;
        public int ActorNumber => photonView.Owner.ActorNumber;

        void Awake()
        {
            photonView = GetComponent<PhotonView>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
