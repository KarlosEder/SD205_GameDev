using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Settings")]
    public float detectionRange = 100.0f;  // How far zombie can "see" player
    public float attackRange = 2.0f;       // How close to attack
    public float attackCooldown = 0.1f;    // Time between combo chains
    public float attackDamage = 10f;

    [Header("Combo Settings")]
    public float comboResetTime = 2.0f;    // Time before combo resets to attack 1

    private NavMeshAgent agent;
    private Target targetScript;
    private Player playerScript;
    private float lastAttackTime;
    private float lastComboTime;
    private int comboStep = 0; // 0 = ready for combo, 1-3 = which attack in combo
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

        // Check if zombie died
        if (targetScript != null && targetScript.health <= 0)
        {
            Die();
            return;
        }

        // Reset combo if too much time passed since last attack
        if (Time.time - lastComboTime > comboResetTime && comboStep > 0)
        {
            comboStep = 0;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update Speed parameter for animation interrupts
        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);

        // STATE 1: Player is within detection range - ENGAGE
        if (distanceToPlayer <= detectionRange)
        {
            // STATE 1A: Close enough to attack
            if (distanceToPlayer <= attackRange)
            {
                // Stop all movement
                agent.isStopped = true;
                agent.ResetPath();

                // Face the player aggressively
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 15f);

                // Attack if not currently attacking and cooldown ready
                if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            // STATE 1B: Detected but not in attack range - CHASE
            else
            {
                // Reset combo if chasing (player got away)
                if (isAttacking)
                {
                    comboStep = 0;
                    isAttacking = false;
                }

                // Make sure zombie is allowed to move
                agent.isStopped = false;

                // Chase the player
                agent.SetDestination(player.position);

                // Face movement direction while running
                if (currentSpeed > 0.1f)
                {
                    Vector3 direction = agent.velocity.normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
        }
        // STATE 2: Player is OUT of detection range - IDLE
        else
        {
            // Reset combo if player left detection range
            if (isAttacking || comboStep > 0)
            {
                comboStep = 0;
                isAttacking = false;
            }

            // Stop moving completely
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        lastComboTime = Time.time;

        // Increment combo step BEFORE triggering
        comboStep++;
        if (comboStep > 3)
            comboStep = 1; // Loop back to first attack

        // Trigger the attack (combo chains happen automatically in animator)
        animator.SetTrigger("AttackTrigger");

        // Calculate duration based on which attack
        float attackDuration = GetAttackDuration(comboStep);

        // Reset attacking flag after this attack finishes
        StartCoroutine(ResetAttack(attackDuration));
    }

    float GetAttackDuration(int step)
    {
        // Adjust these to match your animation lengths
        // If you sped animations to 1.5x, divide these values by 1.5
        switch (step)
        {
            case 1: return 0.6f; // Attack1 duration
            case 2: return 0.6f; // Attack2 duration  
            case 3: return 0.8f; // Attack3 duration (finishing move)
            default: return 0.6f;
        }
    }

    IEnumerator ResetAttack(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Check if this was the last attack in combo
        if (comboStep >= 3)
        {
            // Finished full combo - reset to beginning
            comboStep = 0;
        }

        isAttacking = false;
    }

    // Called by Animation Events on each attack animation
    public void DealDamage()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only deal damage if still in range
        if (distanceToPlayer <= attackRange && playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
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

    // Visualize detection range in editor
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