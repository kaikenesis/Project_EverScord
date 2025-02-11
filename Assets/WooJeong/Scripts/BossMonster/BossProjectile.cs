using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    public void Setup(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    private void Update()
    {
        transform.Translate(direction * speed);
    }

    
}
