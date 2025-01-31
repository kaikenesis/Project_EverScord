using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EverScord
{
    public class UISelect : MonoBehaviour, ISystemSelector, IJobSelector, ILevelSelector
    {
        public static Action OnChangeUserData = delegate { };
        public static Action OnGameStart = delegate { };


        public void OnClickedGameStart()
        {
            OnGameStart?.Invoke();
        }

        public void SelectDealer()
        {
            if (GameManager.Instance.userData.job == EJob.DEALER) return;

            GameManager.Instance.userData.job = EJob.DEALER;
            OnChangeUserData?.Invoke();
        }

        public void SelectHealer()
        {
            if (GameManager.Instance.userData.job == EJob.HEALER) return;

            GameManager.Instance.userData.job = EJob.HEALER;
            OnChangeUserData?.Invoke();
        }

        public void SelectStoryMode()
        {
            if (GameManager.Instance.userData.curLevel == ELevel.STORY) return;

            GameManager.Instance.userData.curLevel = ELevel.STORY;
            OnChangeUserData?.Invoke();
        }

        public void SelectNormalMode()
        {
            if (GameManager.Instance.userData.curLevel == ELevel.NORMAL) return;

            GameManager.Instance.userData.curLevel = ELevel.NORMAL;
            OnChangeUserData?.Invoke();
        }

        public void SelectHardMode()
        {
            if (GameManager.Instance.userData.curLevel == ELevel.HARD) return;

            GameManager.Instance.userData.curLevel = ELevel.HARD;
            OnChangeUserData?.Invoke();
        }
    }
}
