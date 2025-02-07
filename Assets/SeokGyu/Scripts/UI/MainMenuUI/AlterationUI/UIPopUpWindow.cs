using System;
using TMPro;
using UnityEngine;

public class UIPopUpWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text mainMessage;
    [SerializeField] private TMP_Text subMessage;

    /*
    ���� ��ȭ�� �ʿ����� Textǥ��, �� ���� ��ȭ�� ������ Textǥ�� (�����ϸ� �� ����)
    ��ȭ�Ҹ� �����̸� Ȯ�� ������ ��ȭ ����

    �����ִ� ���ڿ� �����ɼ� �������� ���� �ɼ� -> ���ο� �ɼ� �̸� ������ ������ Textǥ��
    Ȯ�� ������ ���ο� �ɼ��� �����ϰ� ���� �̹��� ����
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
        // mainMsg, subMsg, �䱸 money, ���� money �ʿ�
        SetMessage("���ڸ� �����ϱ� ���ؼ� ��ȭ�� ???��ŭ �ʿ��մϴ�. �����Ͻðڽ��ϱ�?", "���� ��ȭ : ???");
    }

    public void SetMessage(string mainMessage, string subMessage)
    {
        this.mainMessage.text = mainMessage;
        this.subMessage.text = subMessage;
    }
}
