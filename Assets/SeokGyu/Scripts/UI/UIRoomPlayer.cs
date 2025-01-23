using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIRoomPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        public void Initialize(string name)
        {
            //플레이어 이름, 이미지1(캐릭터 초상화), 이미지2(포지션)
            nameText.text = name;
        }
    }
}
