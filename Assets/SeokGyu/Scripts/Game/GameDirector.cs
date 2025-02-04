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

        private void CreatePlayer()
        {
            //Transform[] points = pointGroup.GetComponentsInChildren<Transform>();
            //int idx = Random.Range(1, points.Length);
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            pool.ResourceCache.Add(playerObj.name, playerObj);
            PhotonNetwork.Instantiate(playerObj.name, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        }
    }
}
