using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.UI
{
    public class GameOverControl : MonoBehaviour
    {
        private const float WAIT_TRANSITION = 3f;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject uiHub;
        [SerializeField] private GameObject victoryText, defeatText;
        [SerializeField] private Animator textAnim;
        [SerializeField] private AnimationClip transitionClip;

        public static Action OnGameOver = delegate { };
        public PhotonView View => photonView;
        public static bool IsDefeat { get; private set; }

        void Awake()
        {
            SetIsDefeat(false);
            GameManager.Instance.InitControl(this);
        }

        [PunRPC]
        private void SyncShowText(bool isVictory)
        {
            victoryText.SetActive(false);
            defeatText.SetActive(false);

            if (isVictory)
                victoryText.SetActive(true);
            else
                defeatText.SetActive(true);

            uiHub.SetActive(true);
            if (isVictory)
                SoundManager.Instance.PlaySound("ClearWinSound");
            else
                SoundManager.Instance.PlaySound("ClearDefeatSound");
            StartCoroutine(Transition(isVictory));
        }

        private IEnumerator Transition(bool isVictory)
        {
            yield return new WaitForSeconds(WAIT_TRANSITION);

            textAnim.Play(transitionClip.name, -1, 0f);
            yield return new WaitForSeconds(transitionClip.length);

            GameManager.Instance.ResultControl.TransitionResults(isVictory);
            yield return new WaitForSeconds(1f);
        }

        public void EnableUI(bool state)
        {
            uiHub.SetActive(state);
        }

        public void ShowGameover(bool isVictory)
        {
            OnGameOver?.Invoke();

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncShowText), RpcTarget.All, isVictory);
        }

        public IEnumerator ShowGameover(bool isVictory, float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            ShowGameover(isVictory);
        }

        public void SetIsDefeat(bool state)
        {
            IsDefeat = state;
        }
    }
}
