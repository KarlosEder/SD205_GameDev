using UnityEngine;

public class Target : MonoBehaviour
{
    public Player player;
    public Gun gun;

    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
        {
            Target target = other.GetComponent<Target>();
            target.health -= gun.damage;
            player.CallItemOnHit(target);
        }
    }
}
