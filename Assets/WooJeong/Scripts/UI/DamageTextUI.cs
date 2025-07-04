using DG.Tweening;
using EverScord.GameCamera;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextUI : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float duration = 1f;
    [SerializeField] float yOffset = 1f;
    private float timer = 0f;
    private TextMeshProUGUI damageText;
    private Transform target;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        transform.rotation = CharacterCamera.CurrentClientCam.transform.rotation;
    }

    private IEnumerator Updating()
    {
        timer = 0f;
        damageText.DOFade(0, duration);
        while (true)
        {
            timer += Time.deltaTime;
            if(timer >= duration)
            {
                target = null;
                ResourceManager.Instance.ReturnToPool(gameObject, "");
                yield break;
            }

            transform.position += speed * Time.deltaTime * Vector3.up;

            yield return null;
        }
    }

    public void DisplayDamage(Transform newTarget, float damage, float yOffset = 0)
    {
        target = newTarget;
        transform.position = target.position + new Vector3(0, this.yOffset + yOffset, 0);
        damageText.text = damage.ToString("F0");
        if (gameObject.activeSelf)
            StartCoroutine(Updating());
    }
}
