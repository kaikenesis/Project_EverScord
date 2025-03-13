using System.Collections;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace EverScord.UI
{
    public class GameOverControl : MonoBehaviour
    {
        private const float WAIT_TRANSITION = 3f;
        [SerializeField] private GameObject uiHub;
        [SerializeField] private TextMeshProUGUI victoryText, defeatText;
        [SerializeField] private Animator textAnim;
        [SerializeField] private AnimationClip transitionClip;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
                ShowText(true);
        }

        public void ShowText(bool isVictory)
        {
            victoryText.gameObject.SetActive(false);
            defeatText.gameObject.SetActive(false);

            if (isVictory)
                victoryText.gameObject.SetActive(true);
            else
                defeatText.gameObject.SetActive(true);

            uiHub.SetActive(true);
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            yield return new WaitForSeconds(WAIT_TRANSITION);

            textAnim.Play(transitionClip.name, -1, 0f);
            yield return new WaitForSeconds(transitionClip.length);

            GameManager.Instance.ResultControl.TransitionResults();
            yield return new WaitForSeconds(1.5f);

            uiHub.SetActive(false);
        }
    }
}
