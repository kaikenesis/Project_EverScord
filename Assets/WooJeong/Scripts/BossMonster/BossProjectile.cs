using EverScord;
using EverScord.Character;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private int lifeTime = 2;
    private float curTime = 0;

    public void Setup(Vector3 position, Vector3 direction, float speed)
    {
        transform.position = position;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        this.direction = direction;
        this.speed = speed;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (curTime < lifeTime)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            curTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        curTime = 0;
        ResourceManager.Instance.ReturnToPool(gameObject, "BossProjectile");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if(other.gameObject.CompareTag("Player"))
        {
            CharacterControl controller = other.GetComponent<CharacterControl>();
            controller.DecreaseHP(10);
        }
        if(!other.gameObject.CompareTag("Projectile"))
        {
            ResourceManager.Instance.ReturnToPool(gameObject, "BossProjectile");
        }
    }
}
