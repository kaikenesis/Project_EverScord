using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIRoomPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        public void Initialize(string name)
        {
            //�÷��̾� �̸�, �̹���1(ĳ���� �ʻ�ȭ), �̹���2(������)
            nameText.text = name;
        }
    }
}
