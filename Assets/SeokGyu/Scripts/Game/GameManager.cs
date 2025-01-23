using Photon.Pun;
using UnityEngine;

namespace EverScord
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public string userName;
        public PlayerData userData;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetUserName(EJob curJob, ELevel curLevel)
        {
            PlayerPrefs.SetString("USERNAME", userName + "|" + curJob.ToString() + "|" + curLevel.ToString());
            PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");
        }
    }
}