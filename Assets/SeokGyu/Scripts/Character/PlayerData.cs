namespace EverScord
{
    public enum ECharacter
    {
        NED,
        UNI,
        US,
        MAX
    }

    public enum EJob
    {
        DEALER,
        HEALER,
        MAX
    }

    public enum ELevel
    {
        STORY,
        NORMAL,
        HARD,
        MAX
    }

    public enum EPhotonState
    {
        NONE,
        MATCH,
        STOPMATCH,
        FOLLOW,
        MAX
    }

    public class PlayerData
    {
        public ECharacter character = ECharacter.NED;
        public EJob job = EJob.DEALER;
        public ELevel curLevel = ELevel.NORMAL;
        public EPhotonState curPhotonState = EPhotonState.NONE;
        public int money = 0;
    }
}
