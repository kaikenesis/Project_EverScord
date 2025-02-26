using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogPlane : MonoBehaviour
{
    private float yOffset = 2;

    private void OnEnable()
    {
        StartCoroutine(Up());
    }

    private IEnumerator Up()
    {
        float curY = 0;
        while (true)
        {
            curY += Time.deltaTime * 2;
            transform.position = new Vector3(transform.position.x, curY, transform.position.z);
            if (curY > yOffset)
                yield break;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public void DownFog()
    {
        StartCoroutine(Down());
    }

    private IEnumerator Down()
    {
        float curY = yOffset;
        while (true)
        {
            curY -= Time.deltaTime * 2;
            transform.position = new Vector3(transform.position.x, curY, transform.position.z);
            if (curY < 0)
            {
                gameObject.SetActive(false);
                yield break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
