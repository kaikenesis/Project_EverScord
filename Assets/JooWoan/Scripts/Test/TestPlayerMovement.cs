using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody playerRb;

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        playerRb.AddForce(movement * speed / Time.deltaTime);
    }

}
