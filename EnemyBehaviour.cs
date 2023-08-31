using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent),typeof(Animator))]
public class EnemyBehaviour : MonoBehaviour
{
    public int damageAmount;
    public float healthEnemy = 100f;
    public float attackRate = 2f;
    public float recoveryRate;//nakon kojeg vremena se moze ponovo kretati
    public int scorePoints;

    [SerializeField]
    GameObject player;
    NavMeshAgent agent;
    Animator animator;

    public ParticleSystem deathParticle;
    public ParticleSystem onHitParticle;//(NE KORISTI SE VISE)
    public ParticleSystem spawnParticle;

    public GameObject heal;//izbacuje Item koju heal-a player-a kada ga pokupi
    public GameObject hurtBox;//Gameobject s Collider-om s kojim radim damage player-u
    public Transform spawnParticlePoint;//pozicija iz koje se instancira onHitParticle (NE KORISTI SE VISE)

    bool isAlive=true;

    public float closeCombatDistance = 2f;

    [SerializeField] bool isAttacking = false;
    [SerializeField] bool isRecovered=true;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent=GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        hurtBox.SetActive(false);
        Instantiate(spawnParticle,spawnParticlePoint.position,Quaternion.identity);
}

    private void Update()
    {
        if (!isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);//za provjeru udaljenosti od Player-a

            if (distanceToPlayer > closeCombatDistance && isRecovered)
            {
                agent.SetDestination(player.transform.position);
                animator.SetBool("Walk Forward",true);
            }
            else
            {
                agent.ResetPath();
                animator.SetBool("Walk Forward", false);

                if (isAlive && distanceToPlayer <= closeCombatDistance)
                {
                    isAttacking = true;
                    hurtBox.SetActive(true);
                    AttackPlayer();

                }
            }
        }

        if (healthEnemy <= 0)
        {
            Die();
        }

    }


    void ResetAttack()
    {
        isAttacking=false;
    }

    void ResetRecovery()
    {
        isRecovered = true;
    }

    #region Animation Events
    void AttackPlayer()
    {
        if(isAlive)
        {
            animator.SetTrigger("Basic Attack");
        }
    }
    void ResetAttackTrigger()
    {
        animator.ResetTrigger("Basic Attack");
        Invoke("ResetAttack", attackRate);
        hurtBox.SetActive(false);
    }

    public void ReceiveDamage(float damage)
    {
        healthEnemy -= damage;
        animator.SetBool("Walk Forward", false);
        agent.velocity = Vector3.zero;
        isRecovered = false;
        Invoke(nameof(ResetRecovery), recoveryRate);

    }

    public void OnHitParticle()
    {
        if (isAlive)
        {
            //Instantiate(onHitParticle, hitPoint.position, hitPoint.rotation);
            animator.SetTrigger("Take Damage");
            agent.velocity = Vector3.zero;

        }
    }

    void Die()
    {
        isAlive = false;
        animator.SetBool("Die Game",true);
        gameObject.GetComponent<Collider>().enabled = false;
        agent.speed = 0;

    }

    #endregion

    void DestroyThis()
    {
        GameManager.instance.ScoreUpdate(scorePoints);
        GameManager.instance.killedEnemies++;
        Instantiate(deathParticle, transform.position, Quaternion.identity);

        float healObjectRNG = Random.Range(0, 10);
        if (healObjectRNG >= 3)
        {
            Instantiate(heal, new Vector3(transform.position.x,transform.position.y+1f,transform.position.z), Quaternion.identity);
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)//koristi hurtBox collider, treba premjestit u skriptu na hurtBox-u
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
    }
}
