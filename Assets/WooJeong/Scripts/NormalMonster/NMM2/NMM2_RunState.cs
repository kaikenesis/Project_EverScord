using System.Collections;

public class NMM2_RunState : NRunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }

    protected override IEnumerator Updating()
    {
        navMeshAgent.stoppingDistance = monsterController.monsterData.StopDistance;
        return base.Updating();
    }
}
