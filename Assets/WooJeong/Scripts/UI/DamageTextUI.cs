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
        while (true)
        {
            timer += Time.deltaTime;
            if(timer >= duration)
            {
                target = null;
                damageText.DOFade(0, 0.5f);
                yield return new WaitForSeconds(0.5f);
                ResourceManager.Instance.ReturnToPool(gameObject, "");
                yield break;
            }

            transform.position += speed * Time.deltaTime * Vector3.up;

            yield return null;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void DisplayDamage(Transform newTarget, float damage)
    {
        target = newTarget;
        transform.position = target.position + new Vector3(0, yOffset, 0);
        damageText.text = damage.ToString();
        StartCoroutine(Updating());
    }
}
