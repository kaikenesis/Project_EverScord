using System;
using TMPro;
using UnityEngine;

public class UIPopUpWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text mainMessage;
    [SerializeField] private TMP_Text subMessage;

    /*
    얼마의 재화가 필요한지 Text표시, 내 보유 재화가 얼마인지 Text표시 (부족하면 색 변경)
    재화소모 관련이면 확인 누르면 재화 연산

    열려있는 인자에 랜덤옵션 돌릴때는 기존 옵션 -> 새로운 옵션 이를 적용할 것인지 Text표시
    확인 누를시 새로운 옵션을 적용하고 인자 이미지 변경
    */
    private void Awake()
    {
        UIFactor.OnClickedLockedSlot += HandleClickedLockedSlot;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UIFactor.OnClickedLockedSlot -= HandleClickedLockedSlot;
    }

    private void HandleClickedLockedSlot()
    {
        gameObject.SetActive(true);
        // mainMsg, subMsg, 요구 money, 현재 money 필요
        SetMessage("인자를 개방하기 위해서 재화가 ???만큼 필요합니다. 개방하시겠습니까?", "보유 재화 : ???");
    }

    public void SetMessage(string mainMessage, string subMessage)
    {
        this.mainMessage.text = mainMessage;
        this.subMessage.text = subMessage;
    }
}
