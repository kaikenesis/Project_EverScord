using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDebuffUI : MonoBehaviour
{
    [SerializeField] private GameObject slowUI;
    [SerializeField] private GameObject poisonUI;
    private Image slowImage;
    private Image poisonImage;

    private void Awake()
    {
        slowImage = slowUI.GetComponent<Image>();
        poisonImage = poisonUI.GetComponent<Image>();
    }

    public void DebuffEnter(BossDebuff bossDebuff)
    {
        switch (bossDebuff)
        {
            case BossDebuff.SLOW:
                slowUI.SetActive(true);
                slowImage.DOFade(1, 1);
                break;
            case BossDebuff.POISON:
                poisonUI.SetActive(true);
                poisonImage.DOFade(255, 1);
                break;
        }
    }

    public void DebuffEnd(BossDebuff bossDebuff)
    {
        switch (bossDebuff)
        {
            case BossDebuff.SLOW:
                //slowUI.SetActive(false);
                slowImage.DOFade(0, 1);
                break;
            case BossDebuff.POISON:
                //poisonUI.SetActive(false);
                poisonImage.DOFade(0, 1);
                break;
        }
    }
}
