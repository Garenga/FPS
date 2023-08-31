using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBoss : MonoBehaviour//Trigger-u na pocetku Boss arene
{
    public BossBehaviour boss;
    public GameObject portal;
    public Transform bossRespawnPoint;//kada umrem spawn-a me unutar Boss arene
    public GameObject UIbossHP;

    private void Start()
    {
        UIbossHP.SetActive(false);
        boss.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            UIbossHP.SetActive(true);
            boss.enabled = true;
            portal.SetActive(false);
            GameManager.instance.respawnPosition = bossRespawnPoint;
            Destroy(gameObject);

        }
    }
}
