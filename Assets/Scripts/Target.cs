using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Player player;
    public Gun gun;

    public GameObject damageNumbers;
    public Canvas uiCanvas;

    public float health = 50f;
    private Coroutine burnCoroutine;

    private List<DamageNumbers> activeDamageTexts = new List<DamageNumbers>();

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, false); 
    }

    public void TakeDamage(float amount, bool isBurning = false)
    {
        health -= amount;

        // TEST
        Debug.Log($"Took {amount} damage. Health: {health} | Burn: {isBurning}");

        Debug.Log($"Spawning damage number: {amount}, burn={isBurning}");

        if (damageNumbers != null && uiCanvas != null)
        {
            GameObject dmgTextGO = Instantiate(damageNumbers, uiCanvas.transform);
            DamageNumbers dt = dmgTextGO.GetComponent<DamageNumbers>();

            Vector3 spawnOffset = Vector3.up * 2f;
            dt.Initialize(transform, amount.ToString("0"), spawnOffset, isBurning);
        }

        if (health <= 0)
            Die();
    }

    public void ApplyBurn(float damagePerTick, float duration, float tickRate)
    {
        // Restart burn if already burning
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine = StartCoroutine(BurnRoutine(damagePerTick, duration, tickRate));
    }

    IEnumerator BurnRoutine(float damage, float duration, float tickRate)
    {
        float elapsed = 0f;

        while (elapsed < duration && health > 0)
        {
            TakeDamage(damage, true);
            elapsed += tickRate;
            yield return new WaitForSeconds(tickRate);
        }

        burnCoroutine = null;

        // TEST
        Debug.Log($"Burn tick: {damage} damage at elapsed {elapsed}");
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
