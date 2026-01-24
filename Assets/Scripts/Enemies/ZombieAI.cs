using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Settings")]
    public float detectionRange = 100.0f;
    public float attackRange = 2.0f;
    public float attackCooldown = 0.1f;
    public float attackDamage = 10f;

    private NavMeshAgent agent;
    private Target targetScript;
    private Player playerScript;
    private float lastAttackTime;
    private bool isDead = false;
    private bool inAttackRange = false;
    private float effectiveAttackRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        targetScript = GetComponent<Target>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerScript = player.GetComponent<Player>();
            }
        }
        else
        {
            playerScript = player.GetComponent<Player>();
        }

        float zombieHeight = GetComponent<Collider>().bounds.size.y;
        float heightFactor = Mathf.Max(1f, 2f / zombieHeight);
        effectiveAttackRange = attackRange * heightFactor;

        agent.stoppingDistance = 0.5f;
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        if (targetScript != null && targetScript.health <= 0)
        {
            Die();
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );

        // Always chase player
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Check if in attack range
        bool wasInRange = inAttackRange;
        inAttackRange = (distanceToPlayer <= effectiveAttackRange + 0.5f);

        float currentSpeed = agent.velocity.magnitude;

        if (inAttackRange)
        {
            // IN RANGE - set Speed LOW (but not 0) so attacks play but zombie can still shuffle
            animator.SetFloat("Speed", 0.3f);

            // Face player
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;
            
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 15f);
            }

            // Trigger attack once when entering range
            if (!wasInRange && Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("AttackTrigger");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // OUT OF RANGE - set Speed to actual velocity (will be > 1.0 when running)
            animator.SetFloat("Speed", currentSpeed);

            // Face movement direction
            if (currentSpeed > 0.1f)
            {
                Vector3 direction = agent.velocity.normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    public void DealDamage()
    {
        float distanceToPlayer = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );

        if (distanceToPlayer <= effectiveAttackRange + 0.5f && playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);

        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        this.enabled = false;

        DropRateManager dropManager = GetComponent<DropRateManager>();
        if (dropManager != null)
            dropManager.SpawnDrop();

        Destroy(gameObject, 5f);
    }
}