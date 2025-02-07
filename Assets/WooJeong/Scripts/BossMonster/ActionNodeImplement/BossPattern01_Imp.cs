using System.Collections;
using UnityEngine;

public class BossPattern01_Imp : ActionNodeImplement
{
    private float projectileSize = 1;
    private float projectileSpeed = 1;

    protected override IEnumerator Action()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("Attack1 start");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 7; i++)
        {
            Fire();
            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);

        isEnd = true;
        action = null;
        Debug.Log("Attack1 end");
    }

    private void Fire()
    {
        GameObject attackObj = ResourceManager.Instance.InstantiatePrefab("BossProjectile", transform.position).Result;
        BossProjectile projectile = attackObj.GetComponent<BossProjectile>();
    }
}
