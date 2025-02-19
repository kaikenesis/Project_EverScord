using EverScord;
using EverScord.Character;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int ID {  get; private set; }
    private Vector3 direction;
    private float speed;
    private int lifeTime = 2;
    private float curTime = 0;

    public bool IsDestroyed {  get; private set; }

    private void Awake()
    {
        ID = -1;
        IsDestroyed = false;
    }

    public void SetIsDestroyed(bool isDestroyed)
    {
        this.IsDestroyed = isDestroyed;
    }

    public void Setup(int id, Vector3 position, Vector3 direction, float speed)
    {
        ID = id;
        IsDestroyed = false;
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
        //if (!PhotonNetwork.IsMasterClient)
            //return;
        if(other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        if(other.gameObject.CompareTag("Player"))
        {
            CharacterControl controller = other.GetComponent<CharacterControl>();
            controller.DecreaseHP(10);
        }
        IsDestroyed = true;
    }
}
