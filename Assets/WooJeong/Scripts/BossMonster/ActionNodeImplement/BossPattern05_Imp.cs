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
        Vector3 projectorPos = transform.position + (transform.forward * Mathf.Sqrt(attackRadius * attackRadius)/2);
        bossRPC.PlayAnimation("Idle");
        bossRPC.PlayAnimation("StandingAttack");
        yield return bossRPC.ProjectEnable(5, 1f);
        //yield return new WaitForSeconds(0.5f);
        //충돌 판정
        foreach (var player in GameManager.Instance.playerPhotonViews)
        {
            Vector3 toPlayerVector = (player.transform.position - transform.position).normalized;
            if(toPlayerVector.magnitude > attackRadius)
                continue;

            // '타겟-나 벡터'와 '내 정면 벡터'를 내적
            float dot = Vector3.Dot(toPlayerVector, transform.forward);
            // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
            float theta = Mathf.Acos(dot);
            // angleRange와 비교하기 위해 degree로 변환
            float degree = Mathf.Rad2Deg * theta;

            // 시야각 판별
            if (degree <= 45)
            {
                // 플레이어 충돌 함수 추가
                CharacterControl controller = player.GetComponent<CharacterControl>();
                controller.DecreaseHP(10);
            }
        }
        yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 1.5f);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
    }
}
