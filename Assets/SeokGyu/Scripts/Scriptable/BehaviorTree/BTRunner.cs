using Photon.Pun;
using UnityEngine;

namespace EverScord
{
    public abstract class BTRunner : MonoBehaviour
    {
        [SerializeField] private RootNode root;
        [SerializeField] protected BaseBlackBoard blackboard;

        private void Awake()
        {
            Init();
            root.Init();
            root.SetBlackboard(blackboard);
        }

        private void Update()
        {
            if(PhotonNetwork.IsConnected)
            {
                if (!PhotonNetwork.IsMasterClient)
                    return;

                if (root != null)
                    root.Evalute();
            }
            else
            {
                if (root != null)
                    root.Evalute();
            }
        }

        protected abstract void Init();
    }
}

