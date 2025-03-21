using UnityEngine;

namespace EverScord
{
    public class UIDisplaySkill : MonoBehaviour
    {
        public enum EType
        {
            Skill,
            Hyper
        }

        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private EType[] type;
        [SerializeField] private Sprite[] inputSourceImg;
        private GameObject[] slots;

        private void Awake()
        {
            slots = new GameObject[type.Length];

            for (int i = 0; i < type.Length; i++)
            {
                GameObject obj = Instantiate(slotPrefab, transform);
                slots[i] = obj;
            }       
        }

        private void Start()
        {
            PlayerData.ECharacter curChar;
            if (GameManager.Instance.PlayerData != null)
                curChar = GameManager.Instance.PlayerData.character;
            else
                curChar = PlayerData.ECharacter.Ned;

            Sprite skillSprite = GameManager.Instance.InGameUIData.CharacterDatas[(int)curChar].SkillSourceImg;
            Sprite hyperSprite = GameManager.Instance.InGameUIData.CharacterDatas[(int)curChar].HyperSourceImg;

            for (int i = 0; i < slots.Length; i++)
            {
                switch (type[i])
                {
                    case EType.Skill:
                        slots[i].GetComponent<UISkill>().Initialize((int)type[i], skillSprite, inputSourceImg[i]);
                        break;
                    case EType.Hyper:
                        slots[i].GetComponent<UISkill>().Initialize((int)type[i], hyperSprite, inputSourceImg[i]);
                        break;
                }
            }
        }
    }
}
