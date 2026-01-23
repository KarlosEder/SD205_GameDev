using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ExperienceOrbPhysics : MonoBehaviour
{
    // Forces
    public float upwardForce = 3f;
    public float horizontalForce = 2f;

    // Magnet
    public float magnetRadius = 5f;
    public float magnetSpeed = 10f;

    private Rigidbody rb;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator Start()
    {
        // Prevent terrain penetration on spawn
        rb.isKinematic = true;
        yield return null; // wait one frame
        rb.isKinematic = false;

        ApplyRandomDropForce();

        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void ApplyRandomDropForce()
    {
        Vector3 randomHorizontal = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 force =
            Vector3.up * upwardForce +
            randomHorizontal * horizontalForce;

        rb.AddForce(force, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (distance <= magnetRadius)
        {
            // Move toward player
            Vector3 move = direction.normalized * magnetSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }
}
