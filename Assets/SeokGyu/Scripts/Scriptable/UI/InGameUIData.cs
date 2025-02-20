using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Game/InGameUIData", fileName = "newInGameUIData")]
    public class InGameUIData : ScriptableObject
    {
        [SerializeField] private CharacterData[] characterDatas;
        [SerializeField] private JobData[] jobDatas;

        public CharacterData[] CharacterDatas
        {
            get { return characterDatas; }
            private set { characterDatas = value; }
        }

        public JobData[] JodDatas
        {
            get { return jobDatas; }
            private set { jobDatas = value; }
        }

        [System.Serializable]
        public class CharacterData
        {
            [SerializeField] private Sprite portraitSourceImg;
            [SerializeField] private Sprite skillSourceImg;
            [SerializeField] private Sprite hyperSourceImg;

            public Sprite PortraitSourceImg
            {
                get {  return portraitSourceImg; }
                private set { portraitSourceImg = value; }
            }

            public Sprite SkillSourceImg
            {
                get { return skillSourceImg; }
                private set { skillSourceImg = value; }
            }

            public Sprite HyperSourceImg
            {
                get { return hyperSourceImg; }
                private set { hyperSourceImg = value; }
            }
        }

        [System.Serializable]
        public class JobData
        {
            public enum EType
            {
                Dealer,
                Healer
            }

            [SerializeField] private Sprite iconSourceImg;
            [SerializeField] private EType type;

            public Sprite IconSourceImg
            {
                get { return iconSourceImg; }
                private set { iconSourceImg = value; }
            }

            public EType Type
            {
                get { return type; }
                private set { type = value; }
            }
        }
    }
}
