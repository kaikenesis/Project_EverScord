using UnityEngine;
using EverScord.Effects;

namespace EverScord
{
    public class StunnedDebuff : Debuff
    {
        public int LeftCount;
        private GameObject effect1, effect2;
        private GameObject inEffect, outEffect;

        public StunnedDebuff(int amount)
        {
            effect1 = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.StunnedDebuffEffect_ID);
            effect2 = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.StunnedDebuffOut_ID);
            LeftCount = amount;
        }

        public void DecreaseCount()
        {
            --LeftCount;

            if (LeftCount <= 0)
                RemoveDebuff();
        }

        protected override Debuff ShowDebuffEffect()
        {
            inEffect = Object.Instantiate(effect1, target.PlayerTransform.position, Quaternion.identity);
            target.AnimationControl.SetAnimatorEnabled(false);

            target.BlinkEffects.SetDefaultColor(GameManager.StunnedBlinkInfo.BlinkColor);
            target.BlinkEffects.SetMaterialColors(GameManager.StunnedBlinkInfo.BlinkColor);

            return base.ShowDebuffEffect();
        }

        public override void RemoveDebuff()
        {
            outEffect = Object.Instantiate(effect2, target.PlayerTransform.position, Quaternion.identity);

            Object.Destroy(inEffect);
            target.AnimationControl.SetAnimatorEnabled(true);

            target.BlinkEffects.SetDefaultColor();
            target.BlinkEffects.SetMaterialColors(default, true);

            base.RemoveDebuff();
        }
    }
}