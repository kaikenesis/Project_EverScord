using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    private void Start()
    {
        
    }

    private void Update()
    {
        transform.Translate(direction * speed);
    }

    
}
