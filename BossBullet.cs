using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed;
    public int bossDamage;

    Vector3 location;
    public ParticleSystem onHit;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(bossDamage);
        }
        if (other.gameObject.tag == "Ground")
        {
            var collider = GetComponent<Collider>();

            Vector3 pointOf = collider.ClosestPoint(location);

            Debug.Log(pointOf);
            Instantiate(onHit, pointOf, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

}
