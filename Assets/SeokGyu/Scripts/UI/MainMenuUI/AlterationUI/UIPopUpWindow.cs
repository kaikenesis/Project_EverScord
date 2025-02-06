using TMPro;
using UnityEngine;

public class UIPopUpWindow : MonoBehaviour
{
    enum Type
    {
        UNLOCK_FACTOR,
        ROLL_FACTOR,
        DECLINE_FACTOR,
        MAX
    }

    [SerializeField] private TMP_Text mainMessage;
    [SerializeField] private TMP_Text subMessage;

    /*
    ���� ��ȭ�� �ʿ����� Textǥ��, �� ���� ��ȭ�� ������ Textǥ�� (�����ϸ� �� ����)
    ��ȭ�Ҹ� �����̸� Ȯ�� ������ ��ȭ ����

    �����ִ� ���ڿ� �����ɼ� �������� ���� �ɼ� -> ���ο� �ɼ� �̸� ������ ������ Textǥ��
    Ȯ�� ������ ���ο� �ɼ��� �����ϰ� ���� �̹��� ����
    */

    public void Initialize()
    {

    }

    public void SetMessage(string mainMessage, string subMessage)
    {
        this.mainMessage.text = mainMessage;
        this.subMessage.text = subMessage;
    }
}
