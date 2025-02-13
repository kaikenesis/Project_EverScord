using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern05_Imp : ActionNodeImplement
{
    private float attackRadius = 10;
    protected override IEnumerator Act()
    {
        Vector3 projectorPos = transform.position + (transform.forward * Mathf.Sqrt(attackRadius * attackRadius)/2);
        bossRPC.PlayAnimation("Idle");
        yield return bossRPC.ProjectEnable(2, 1.5f);
        bossRPC.PlayAnimation("StandingAttack");

        yield return new WaitForSeconds(0.5f);
        //�浹 ����
        foreach (var player in GameManager.Instance.playerPhotonViews)
        {
            Vector3 toPlayerVector = (player.transform.position - transform.position).normalized;
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
                CharacterController controller = player.GetComponent<CharacterController>();
                //controller.DereaseHP(10);
            }
        }

        yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 0.5f);
        isEnd = true;
        action = null;
    }
}
