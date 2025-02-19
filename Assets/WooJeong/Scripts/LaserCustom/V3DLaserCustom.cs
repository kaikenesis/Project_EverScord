using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3DLaserCustom : MonoBehaviour
{
    //public Transform targetCursor;
    public float speed = 1f;

    private Vector3 mouseWorldPosition;

    // Positioning cursor prefab
    void FixedUpdate()
    {
        mouseWorldPosition = transform.position + transform.forward * 10;

        Quaternion toRotation = Quaternion.LookRotation(mouseWorldPosition - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
        //targetCursor.position = mouseWorldPosition;
    }
}
