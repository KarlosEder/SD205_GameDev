using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Player player;
    public Gun gun;

    public GameObject damageNumbers;
    public Canvas uiCanvas;

    public float health = 50f;

    private List<DamageNumbers> activeDamageTexts = new List<DamageNumbers>();

    public void TakeDamage(float amount)
    {
        // Subtract health
        health -= amount;

        // Spawn damage text
        if (damageNumbers != null && uiCanvas != null)
        {
            GameObject dmgTextGO = Instantiate(damageNumbers, uiCanvas.transform);
            DamageNumbers dt = dmgTextGO.GetComponent<DamageNumbers>();

            // Calculate a vertical offset based on how many active damage numbers exist
            Vector3 spawnOffset = Vector3.up * (2f + activeDamageTexts.Count * 0.3f);

            dt.Initialize(transform, amount.ToString("0"), spawnOffset);

            activeDamageTexts.Add(dt);

            // Remove from list when destroyed
            dt.OnDestroyed += () => activeDamageTexts.Remove(dt);
        }

        // Check if target is dead
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
        // Detach active damage numbers so they can finish fading
        foreach (DamageNumbers dt in activeDamageTexts)
        {
            dt.Detach();
        }

        if (other.tag == "Target")
        {
            Target target = other.GetComponent<Target>();
            target.health -= gun.damage;
            player.CallItemOnHit(target);
        }
    }
}
