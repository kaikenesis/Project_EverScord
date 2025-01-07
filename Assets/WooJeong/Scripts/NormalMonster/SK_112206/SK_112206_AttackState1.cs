using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SK_112206_AttackState1 : MonoBehaviour, IState, ICoolDown
{
    private SK_112206_Controller monsterController;
    private BoxCollider boxCollider;

    private float curCool;
    public float CurCool { get { return curCool; } }
   
    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void Enter()
    {
        StartCoroutine(Attack1());
    }

    IEnumerator CoolDown()
    {
        curCool = monsterController.CoolDown1;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curCool -= 0.1f;
            if (curCool <= 0)
                yield break;
        }
    }

    IEnumerator Attack1()
    {
        monsterController.Animator.Play("Attack1", -1, 0);
        yield return null;
        AnimatorClipInfo[] clip = monsterController.Animator.GetCurrentAnimatorClipInfo(0);
        float time = clip[0].clip.length;
        yield return new WaitForSeconds(time / 3);
        boxCollider.enabled = true;
        yield return new WaitForSeconds(time / 3);
        boxCollider.enabled = false;
        yield return new WaitForSeconds(time / 3);
        StartCoroutine(CoolDown());
        Exit();
    }

    public void Exit()
    {
        monsterController.IdleState();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
