using System.Collections.Generic;

namespace EverScord.Augment
{
    public abstract class ArmorAugment
    {
        public string Name              { get; protected set; }
        public string Description       { get; protected set; }
        public float DescriptionValue   { get; protected set; }
        public int BonusIndex           { get; protected set; }

        public void SetDescriptionInfo(List<(float, float)> bonusList)
        {
            for (int i = 0; i < bonusList.Count; i++)
            {
                if (bonusList[i].Item1 != 0)
                {
                    DescriptionValue = bonusList[i].Item1;
                    BonusIndex = i;
                    return;
                }

                if (bonusList[i].Item2 != 0)
                {
                    DescriptionValue = bonusList[i].Item2;
                    BonusIndex = i;
                    return;
                }
            }
        }
    }
}
