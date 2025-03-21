using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NMS1_Controller : NController
{
    protected override void Setup()
    {
        monsterType = MonsterType.SMALL;
        runState = gameObject.AddComponent<NMS1_RunState>();
        attackState1 = gameObject.AddComponent<NMS1_AttackState1>();
        attackState2 = gameObject.AddComponent<NMS1_AttackState2>();
        waitState = gameObject.AddComponent<NMS1_WaitState>();
        stunState = gameObject.AddComponent<NMS1_StunState>();
        deathState = gameObject.AddComponent<NMS1_DeathState>();
    }

    public override void StartFSM()
    {
        base.StartFSM();
        PlaySound("NMS1_Spawn");
    }
}
