
public class StatBonus
{
    public float additive = 1f;
    public float multiplicative = 1f;

    public void Init(float additive, float multiplicative)
    {
        this.additive = additive * 0.01f;
        this.multiplicative = (multiplicative + 100f) * 0.01f;
    }

    public float CalculateStat(float statValue)
    {
        return statValue * additive * multiplicative;
    }

    public void MergeBonus(StatBonus newBonus)
    {
        additive += newBonus.additive;
        multiplicative *= newBonus.multiplicative;
    }
}
