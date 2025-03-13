using System;
using UnityEngine;

namespace EverScord
{
    public class UISelect : MonoBehaviour, ISystemSelector, IJobSelector, ILevelSelector
    {
        public static Action OnChangeUserData = delegate { };
        public static Action OnGameStart = delegate { };
        public static Action OnUpdateReady = delegate { };

        public void GameStart()
        {
            OnGameStart?.Invoke();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SelectDealer()
        {
            if (GameManager.Instance.PlayerData.job == PlayerData.EJob.Dealer) return;

            GameManager.Instance.PlayerData.job = PlayerData.EJob.Dealer;
            OnChangeUserData?.Invoke();
        }

        public void SelectHealer()
        {
            if (GameManager.Instance.PlayerData.job == PlayerData.EJob.Healer) return;

            GameManager.Instance.PlayerData.job = PlayerData.EJob.Healer;
            OnChangeUserData?.Invoke();
        }

        public void SelectStoryMode()
        {
            if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Story) return;

            GameManager.Instance.PlayerData.difficulty = PlayerData.EDifficulty.Story;
            OnChangeUserData?.Invoke();
        }

        public void SelectNormalMode()
        {
            if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Normal) return;

            GameManager.Instance.PlayerData.difficulty = PlayerData.EDifficulty.Normal;
            OnChangeUserData?.Invoke();
        }

        public void SelectHardMode()
        {
            if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Hard) return;

            GameManager.Instance.PlayerData.difficulty = PlayerData.EDifficulty.Hard;
            OnChangeUserData?.Invoke();
        }

        public void SetReady(bool bReady)
        {
            GameManager.Instance.PlayerData.bReady = bReady;
            OnUpdateReady?.Invoke();
        }
    }
}
