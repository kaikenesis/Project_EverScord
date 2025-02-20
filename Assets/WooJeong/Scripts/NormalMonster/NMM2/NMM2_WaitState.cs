public class NMM2_WaitState : NWaitState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }
}
