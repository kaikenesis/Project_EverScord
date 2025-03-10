using EverScord.Armor;

public interface IArmor
{
    public static float GetStatChangeAmount(IArmor originalArmor, IArmor upgradedArmor, int bonusIndex)
    {
        float originalStat = 0f;
        float upgradedStat = 0f;

        switch (originalArmor)
        {
            case IHelmet originalHelmet:
            {
                IHelmet upgradedHelmet = upgradedArmor as IHelmet;
                IHelmet.BonusType type = (IHelmet.BonusType)bonusIndex;

                originalStat = originalHelmet.GetStat(type);
                upgradedStat = upgradedHelmet.GetStat(type);
                break;
            }

            case IVest originalVest:
            {
                IVest upgradedVest = upgradedArmor as IVest;
                IVest.BonusType type = (IVest.BonusType)bonusIndex;

                originalStat = originalVest.GetStat(type);
                upgradedStat = upgradedVest.GetStat(type);
                break;
            }

            case IShoes originalShoes:
            {
                IShoes upgradedShoes = upgradedArmor as IShoes;
                IShoes.BonusType type = (IShoes.BonusType)bonusIndex;

                originalStat = originalShoes.GetStat(type);
                upgradedStat = upgradedShoes.GetStat(type);
                break;
            }
        }

        if (originalStat == 0f)
            return -1;

        return (upgradedStat - originalStat) / originalStat * 100f;
    }
}
