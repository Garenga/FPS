using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CombatPhase
{
    Phase1,//Range,No moving, starting phase
    Phase2,//Summons Addons, stops shooting, AT 75% HP
    Phase3,//Shrinks, starts Melee, AT 50% HP
    Phase4//Summons Shield Pylons and Adds, failing to destroy Pylons heals 10% HP, AT 10% HP
}


[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class BossBehaviour : MonoBehaviour
{
    [SerializeField] CombatPhase combatPhase;

    public int damageAmount;
    public float healthBoss;
    public float healthBossMax;
    public float attackRate;
    public float recoveryRate;
    public float moveSpeed;
    public float closeCombatDistance;
    public float timer;
    public float survivetimer;

    public List<GameObject> summonedAdds;
    public List<GameObject> summonedPylons;
    public Image bossHP;
    public GameObject UIbossHP;

    public GameObject bossBullet;

    [SerializeField]
    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    Rigidbody rb;

    public Transform startingPosition;

    public ParticleSystem deathParticle;
    public GameObject hurtBox;
    public Transform hitPoint;
    public Transform shootPoint;

    public GameObject[] addons;
    public GameObject[] pylons;
    public Transform[] addonsSpawnPoint;
    public Transform[] pylonsSpawnPoint;
    public float summonTimer;
    public int destroyedPylons;
    public GameObject shield;

    [SerializeField]
    bool isAttacking = false;
    public bool isRecovered = true;
    bool isInvincible = false;
    bool hasSummonedAddons = false;
    bool hasSummonedPylons = false;
    public bool isAlive = true;
    public int summoned;

    private void Awake()
    {
        healthBossMax = healthBoss;
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        shield.SetActive(false);
        hurtBox.SetActive(false);
        healthBossMax = healthBoss;
        bossHP.fillAmount = healthBossMax / healthBossMax;
        combatPhase = CombatPhase.Phase1;

    }

    private void Update()
    {
        CheckHealth();
        CheckCombatPhases();
        AttackPlayer();

    }

    private void FixedUpdate()
    {
        TrackPlayer();
    }

    void CheckCombatPhases()
    {
        if (combatPhase == CombatPhase.Phase1)
        {
            damageAmount = 18;
            bossBullet.GetComponent<BossBullet>().bossDamage = damageAmount;
            attackRate = 5f;
            moveSpeed = 0f;
            agent.speed = moveSpeed;
            summoned = 25;
        }
        if (combatPhase == CombatPhase.Phase2)
        {
            summonTimer = 3f;
            SummonAddons();

            damageAmount = 18;
            attackRate = 8f;
            moveSpeed = 5f;
            agent.SetDestination(startingPosition.position);

        }
        if (combatPhase == CombatPhase.Phase3)
        {
            timer = 0f;

            StartCoroutine(Shrink());
            damageAmount = 10;
            attackRate = 1f;
            moveSpeed = 5f;
            agent.speed = moveSpeed;
            shield.SetActive(false);
            rb.isKinematic = false;
            isInvincible = false;
            summoned = 12;
            animator.SetBool("Defend", false);
        }
        if (combatPhase == CombatPhase.Phase4)
        {
            SummonPylons();
            SurviveTimer();
            damageAmount = 1;
            attackRate = 1f;
            moveSpeed = 3f;
            agent.speed = moveSpeed;

            transform.position = startingPosition.position;
            rb.isKinematic = true;
            if (summonedPylons.Count>0)
            {
                isInvincible = true;
                shield.SetActive(true);
            }
            else
            {
                isInvincible = false;
                shield.SetActive(false);
                healthBoss = 0f;
            }
            if (shield.activeSelf==true)
            {
                animator.SetBool("Defend", true);
            }
            if (shield.activeSelf==false)
            {
                animator.SetBool("Defend", false);

            }
        }
    }

    void CheckHealth()
    {
        if (healthBoss <= healthBossMax * 0.75f && healthBoss > healthBossMax * 0.50f)
        {
            combatPhase = CombatPhase.Phase2;
        }
        else if (healthBoss <= healthBossMax * 0.50f && healthBoss > healthBossMax * 0.10f)
        {
            combatPhase = CombatPhase.Phase3;
            hasSummonedPylons = false;
        }
        else if (healthBoss <= healthBossMax * 0.10f && healthBoss > 0f)
        {
            combatPhase = CombatPhase.Phase4;
        }
        else if (healthBoss <= 0)
        {
            bossHP.fillAmount = healthBoss / healthBossMax;
            Die();
        }

    }

    void AttackPlayer()
    {
        if (combatPhase == CombatPhase.Phase1|| combatPhase == CombatPhase.Phase2)
        {
            if (!isAttacking)
            {
                Shoot();
                isAttacking = true;
            }
        }

        if (combatPhase == CombatPhase.Phase3)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > closeCombatDistance && isRecovered)
            {
                agent.SetDestination(player.transform.position);
                animator.SetBool("Walk Forward", true);
            }
            else
            {
                agent.ResetPath();
                animator.SetBool("Walk Forward", false);

                if (isAlive && distanceToPlayer <= closeCombatDistance)
                {
                    Debug.Log("Works");
                    isAttacking = true;
                    hurtBox.SetActive(true);
                    MeleeAttack();

                }

            }
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void ResetRecovery()
    {
        isRecovered = true;
    }

    void ResetAttackTrigger()
    {
        animator.ResetTrigger("Basic Attack");
        Invoke(nameof(ResetAttack), attackRate);
        hurtBox.SetActive(false);
    }

    public void ReceiveDamage(float damage)
    {
        if (!isInvincible)
        {
            healthBoss -= damage;
            animator.SetBool("Walk Forward", false);
            animator.SetTrigger("Take Damage");
            agent.velocity = Vector3.zero;
            isRecovered = false;
            Invoke(nameof(ResetRecovery), recoveryRate);
            bossHP.fillAmount = healthBoss / healthBossMax;
        }
    }
    #region Animation Events
    void Shoot()
    {
        animator.SetTrigger("Cast Spell");
    }

    void MeleeAttack()
    {
        animator.SetTrigger("Basic Attack");
    }
    public void OnHitParticle()
    {
        if (isAlive)
        {
            animator.SetTrigger("Take Damage");
            agent.velocity = Vector3.zero;
        }
    }
    #endregion
    #region For Boss Phases
    void SummonProjectile()
    {
        shootPoint.transform.LookAt(player.transform);
        GameObject bullet = Instantiate(bossBullet, shootPoint.position, shootPoint.rotation);
    }

    void SummonAddons()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);

        if (timer >= summonTimer && summoned > 0)
        {
            int randomPosition = Random.Range(0, addonsSpawnPoint.Length);
            int randomEnemy = Random.Range(0, addons.Length);

            var enemy = Instantiate(addons[randomEnemy], addonsSpawnPoint[randomPosition].position, Quaternion.identity);
            enemy.GetComponent<EnemyBehaviour>().healthEnemy = enemy.GetComponent<EnemyBehaviour>().healthEnemy * 0.5f;

            timer = 0;
            summoned--;
            summonedAdds.Add(enemy);//koristim List za disable Enemy kada Boss umre
        }
    }

    void SurviveTimer()
    {

        survivetimer += Time.deltaTime;

        if (survivetimer >= 25)
        {
            healthBoss += (healthBossMax * 0.15f);
            bossHP.fillAmount = healthBoss / healthBossMax;
            survivetimer = 0;

        }
    }

    void SummonPylons()
    {
        if (!hasSummonedPylons)
        {
            if (summonedPylons.Count <= 0)
            {
                for (int i = 0; i < pylons.Length; i++)
                {
                    var pylon=Instantiate(pylons[i], pylonsSpawnPoint[i].position, Quaternion.identity);
                    summonedPylons.Add(pylon);
                }
                hasSummonedPylons = true;

            }
        }
        SummonAddons();
    }
    IEnumerator Shrink()//treba napraviti na drugi nacin
    {
        for(float i = 0;i< 1.5f; i += .001f)
        {
            if (transform.localScale.x > 1.5f)
            {
                transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

                yield return new WaitForSeconds(1f);
            }
            else
            {
                break;
            }
        }
        yield return new WaitForSeconds(1f);
    }
    #endregion
    void TrackPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation.x = 0;
        transform.rotation = rotation;
    }


    void Die()
    {
        UIbossHP.SetActive(false);
        isAlive = false;
        animator.SetBool("Die Game", true);
        agent.speed = 0;
    }

    void DestroyThis()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
    }

    void OnDestroy()
    {

        foreach(GameObject t in summonedAdds)
        {
            t.SetActive(false);
        }
        }

}
