using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylons : MonoBehaviour
{
    public float pylonHealth;

    public GameObject boss;
    BossBehaviour bossBehaviour;

    private void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        bossBehaviour = boss.GetComponent<BossBehaviour>();
    }

   public  void ReceiveDamage(float damageTaken)
    {
        pylonHealth -= damageTaken;
        if (pylonHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        boss.GetComponent<BossBehaviour>().summonedPylons.Remove(this.gameObject);
        bossBehaviour.destroyedPylons++;
        Destroy(this.gameObject);
    }
}
