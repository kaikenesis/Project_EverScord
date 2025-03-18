using System;
using UnityEngine;

namespace EverScord
{
    public class UISelect : MonoBehaviour
    {
        public static Action OnChangeUserData = delegate { };
        public static Action OnChangeCharacter = delegate { };
        public static Action OnGameStart = delegate { };
        public static Action OnUpdateReady = delegate { };

        public void GameStart()
        {
            OnGameStart?.Invoke();
            SoundManager.Instance.PlaySound("ButtonGameStartSound");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SelectNed()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Ned)
                return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Ned;
            OnChangeCharacter?.Invoke();
        }

        public void SelectUni()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Uni)
                return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Uni;
            OnChangeCharacter?.Invoke();
        }

        public void SelectUs()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Us)
                return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Us;
            OnChangeCharacter?.Invoke();
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
