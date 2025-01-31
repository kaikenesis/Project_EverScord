using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private GameObject playerObj;

        private void Awake()
        {
            CreatePlayer();
        }

        private void Start()
        {
            Debug.Log(PhotonNetwork.PlayerList.Length);
            foreach (var p in PhotonNetwork.PlayerList)
            {
                int actorNr = p.ActorNumber;
                for (int viewId = actorNr * PhotonNetwork.MAX_VIEW_IDS + 1; viewId < (actorNr + 1) * PhotonNetwork.MAX_VIEW_IDS; viewId++)
                {
                    Debug.Log(viewId);
                    PhotonView photonView = PhotonView.Find(viewId);
                    if (photonView)
                    {
                        GameManager.Instance.playerPhotonViews.Add(photonView);
                        break;
                    }
                }
            }
        }

        private void CreatePlayer()
        {
            //Transform[] points = pointGroup.GetComponentsInChildren<Transform>();
            //int idx = Random.Range(1, points.Length);
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            pool.ResourceCache.Add("Player", playerObj);
            PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        }
    }
}
