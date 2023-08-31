using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    [SerializeField] float aliveTime = 2f;
    public ParticleSystem onHit;

    public float damage = 10;
    Vector3 location;

    private void Update()
    {

        aliveTime-=Time.deltaTime;
        if (aliveTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyBehaviour>().ReceiveDamage(damage);
            other.gameObject.GetComponent<EnemyBehaviour>().OnHitParticle();
            var collider = GetComponent<Collider>();

            Vector3 pointOf = collider.ClosestPoint(location);

            Debug.Log(pointOf);
            Instantiate(onHit, pointOf, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (other.gameObject.tag == "Boss")
        {
            other.gameObject.GetComponent<BossBehaviour>().ReceiveDamage(damage);
            other.gameObject.GetComponent<BossBehaviour>().OnHitParticle();

            var collider = GetComponent<Collider>();

            Vector3 pointOf = collider.ClosestPoint(location);

            Debug.Log(pointOf);
            Instantiate(onHit, pointOf, Quaternion.identity);
            Destroy(this.gameObject);

        }
        if (other.gameObject.tag == "Pylon")
        {
            other.gameObject.GetComponent<Pylons>().ReceiveDamage(damage);

            var collider = GetComponent<Collider>();

            Vector3 pointOf = collider.ClosestPoint(location);

            Debug.Log(pointOf);
            Instantiate(onHit, pointOf, Quaternion.identity);
            Destroy(this.gameObject);

        }

        if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Shield")
        {
            var collider = GetComponent<Collider>();

            Vector3 pointOf = collider.ClosestPoint(location);

            Debug.Log(pointOf);
            Instantiate(onHit, pointOf, Quaternion.identity);
            Destroy(this.gameObject);
        }

        if (other.gameObject.tag == "BossProjectile")
        {
            other.gameObject.GetComponent<BossBullet>().Die();

            var collider = GetComponent<Collider>();

            Vector3 pointOf = collider.ClosestPoint(location);

            Debug.Log(pointOf);
            Instantiate(onHit, pointOf, Quaternion.identity);
            Destroy(this.gameObject);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground"|| collision.gameObject.tag == "Shield")
        {
         Debug.Log(collision.contacts[0].point);

        }
    }
}
