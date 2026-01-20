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

        // Debug status check
        Invoke("DebugNavMeshStatus", 1f);
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        // Check if zombie died
        if (targetScript != null && targetScript.health <= 0)
        {
            Die();
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // STATE 1: Player is within detection range - ENGAGE
        if (distanceToPlayer <= detectionRange)
        {
            // STATE 1A: Close enough to attack
            if (distanceToPlayer <= attackRange)
            {
                // Stop all movement
                agent.isStopped = true;
                agent.ResetPath(); // Clear any path

                // Face the player
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

                // Set speed to 0 for idle animation
                animator.SetFloat("Speed", 0f);

                // Attack if not currently attacking and cooldown ready
                if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            // STATE 1B: Detected but not in attack range - CHASE
            else
            {
                // Make sure zombie is allowed to move
                agent.isStopped = false;

                // Chase the player
                agent.SetDestination(player.position);

                // Face movement direction while running
                if (agent.velocity.magnitude > 0.1f)
                {
                    Vector3 direction = agent.velocity.normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

                // Set animator speed based on actual movement
                animator.SetFloat("Speed", agent.velocity.magnitude);

                Debug.Log($"CHASING - Distance: {distanceToPlayer:F1}m | Speed: {agent.velocity.magnitude:F2}");
            }
        }
        // STATE 2: Player is OUT of detection range - IDLE
        else
        {
            // Stop moving completely
            agent.isStopped = true;
            agent.ResetPath(); // Clear any destination

            // Set to idle animation
            animator.SetFloat("Speed", 0f);

            Debug.Log($"IDLE - Player too far: {distanceToPlayer:F1}m");
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetTrigger("AttackTrigger");

        Debug.Log("ATTACKING!");

        // Reset attacking flag after animation duration
        // Adjust to match the attack animation length if needed - Jack
        StartCoroutine(ResetAttack(2.0f));
    }

    IEnumerator ResetAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    // Called by Animation Event on the attack animation
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

        // Stop all movement
        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;

        // Disable collider so player can walk through
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Disable this script
        this.enabled = false;

        // Destroy after death animation finishes
        Destroy(gameObject, 5f);
    }

    // Debug NavMesh status
    void DebugNavMeshStatus()
    {
        Debug.Log("=================================");
        Debug.Log($"ZOMBIE POSITION: {transform.position}");
        Debug.Log($"IS ON NAVMESH: {agent.isOnNavMesh}");
        Debug.Log($"AGENT ENABLED: {agent.enabled}");

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            Debug.Log($"DISTANCE TO PLAYER: {dist:F1}m");
            Debug.Log($"SHOULD BE CHASING: {dist <= detectionRange}");
        }

        Debug.Log("=================================");

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("PROBLEM: Zombie is NOT on NavMesh!");
        }
    }

    //Visualize detection range in editor
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}