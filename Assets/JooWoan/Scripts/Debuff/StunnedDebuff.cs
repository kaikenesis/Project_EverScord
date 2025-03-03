
namespace EverScord
{
    public class StunnedDebuff : Debuff
    {
        public int LeftCount;

        public StunnedDebuff(int amount)
        {
            LeftCount = amount;
        }

        public void DecreaseCount()
        {
            --LeftCount;

            if (LeftCount <= 0)
                Release();
        }

        public override void Release()
        {
            
        }
    }
}