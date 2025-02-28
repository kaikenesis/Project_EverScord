using EverScord;
using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern05_Imp : ActionNodeImplement
{
    private float attackRadius = 10;
    protected override IEnumerator Act()
    {
        // Vector3 projectorPos = transform.position + (transform.forward * Mathf.Sqrt(attackRadius * attackRadius)/2);
        bossRPC.PlayAnimation("Idle");
        bossRPC.PlayAnimation("StandingAttack");
        yield return bossRPC.ProjectEnable(5, 1f);
        //yield return new WaitForSeconds(0.5f);
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
