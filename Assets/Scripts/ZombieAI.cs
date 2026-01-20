using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Settings")]
    public float detectionRange = 15.0f;  // How far zombie can "see" player
    public float attackRange = 2.0f;       // How close to attack
    public float attackCooldown = 2.0f;    // Time between attacks
    public float attackDamage = 10f;

    private NavMeshAgent agent;
    private Target targetScript;
    private Player playerScript;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        // Get components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        targetScript = GetComponent<Target>();

        // Find the player automatically
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

        // Set stopping distance to match attack range
        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        // Check if zombie has died (using Target script's health)
        if (targetScript != null && targetScript.health <= 0)
        {
            Die();
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only chase if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // If in attack range
            if (distanceToPlayer <= attackRange)
            {
                // Stop moving
                agent.isStopped = true;

                // Face the player
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

                // Set speed to 0 for idle animation
                animator.SetFloat("Speed", 0f);

                // Attack if cooldown is ready
                if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // Player detected but not in attack range - CHASE
                agent.isStopped = false;

                // Move toward player
                agent.SetDestination(player.position);

                // Set animator speed based on movement
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }
        else
        {
            // Player out of detection range - idle
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetTrigger("AttackTrigger");

        // Reset attacking flag after animation duration
        // Adjust 2.0f to match the actual attack animation length/as needed - Jack
        StartCoroutine(ResetAttack(2.0f));
    }

    IEnumerator ResetAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    // This method will be called by an Animation Event
    public void DealDamage()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only deal damage if still in range
        if (distanceToPlayer <= attackRange && playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
            Debug.Log($"Zombie dealt {attackDamage} damage to player!");
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);

        // Stop moving
        agent.isStopped = true;
        agent.enabled = false;

        // Disable collider so player can walk through
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Disable this script
        this.enabled = false;

        // Destroy after animation finishes
        Destroy(gameObject, 5f);
    }
}