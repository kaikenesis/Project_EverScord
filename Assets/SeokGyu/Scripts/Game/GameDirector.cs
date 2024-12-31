using Photon.Pun;
using UnityEngine;

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
        pool.ResourceCache.Add("Player", playerObj);
        PhotonNetwork.Instantiate("Player", new Vector3(0,0,0), new Quaternion(0,0,0,0));
    }
}
