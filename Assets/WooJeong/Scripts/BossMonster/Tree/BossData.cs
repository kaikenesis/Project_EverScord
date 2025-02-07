using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Data/BossData")]
public class BossData : ScriptableObject
{
    public float HP { get; protected set; }
    public int Phase { get; protected set; }
    [field: SerializeField] public float MaxHP { get; protected set; }
    [field: SerializeField] public float Speed { get; protected set; }
    [field: SerializeField] public float StopDistance { get; protected set; }
    [field: SerializeField] public float AttackRange { get; protected set; }

    public void ResetParams()
    {       
        HP = MaxHP;
        Phase = 1;
    }

    public void ReduceHp(int decrease)
    {
        HP -= decrease;
        if (HP < 0) 
            HP = 0;
        Debug.Log(decrease + " 데미지, 남은 체력 : " + HP);
    }

    public void PhaseUp()
    {
        Debug.Log("2페이즈 진입");
        Phase++;
        HP = 50;
    }

    public bool IsUnderHP90()
    {
        if (HP >= MaxHP/100 * 90)
        {
            return false;
        }
        return true;
    }
}
