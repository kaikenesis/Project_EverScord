using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPattern2_Attack : MonoBehaviour
{
    private float attackDamage = 10;

    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterControl controller = other.GetComponent<CharacterControl>();
            controller.DecreaseHP(attackDamage);
        }
    }
}
