using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealObjects : MonoBehaviour
{
    public int healAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().HealDamage(healAmount);
            Destroy(this.gameObject);
        }
    }
}
