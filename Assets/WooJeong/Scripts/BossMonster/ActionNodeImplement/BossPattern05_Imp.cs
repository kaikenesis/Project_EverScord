using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern05_Imp : AttackNodeImplement
{
    private float attackRadius = 10;

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 80;
    }

    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("Idle");
        bossRPC.PlayAnimation("StandingAttack");
        yield return bossRPC.ProjectEnable(5, 1f);
        bossRPC.PlayEffect("StandingAttackEffect", transform.position + transform.forward * 3);
        //�浹 ����
        foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
        {
            Vector3 toPlayerVector = (player.PlayerTransform.position - transform.position).normalized;
            if(toPlayerVector.magnitude > attackRadius)
                continue;

            // 'Ÿ��-�� ����'�� '�� ���� ����'�� ����
            float dot = Vector3.Dot(toPlayerVector, transform.forward);
            // �� ���� ��� ���� �����̹Ƿ� ���� ����� cos�� ���� ���ؼ� theta�� ����
            float theta = Mathf.Acos(dot);
            // angleRange�� ���ϱ� ���� degree�� ��ȯ
            float degree = Mathf.Rad2Deg * theta;

            // �þ߰� �Ǻ�
            if (degree <= 45)
            {
                // �÷��̾� �浹 �Լ� �߰�
                player.DecreaseHP(10);
            }
        }
        yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 1.5f);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
    }
}
