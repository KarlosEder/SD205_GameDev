using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Set health 
    public float maxHealth = 100f;
    public float currentHealth;

    // Set shield
    public float maxShield = 100f;
    public float currentShield;

    // Set shield recharge
    public float rechargeDelay = 3f;
    public float rechargeRate = 40f;

    private Coroutine rechargeCoroutine;

    public HealthBar healthBar;
    public HealthBar shieldBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentShield = maxShield;
        shieldBar.SetMaxShield(maxShield);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(float damage)
    {   
        // Reset recharge when damaged
        if (rechargeCoroutine != null) StopCoroutine(rechargeCoroutine); 

        // Check shield amount
        if (currentShield > 0)
        {
            if (currentShield >= damage)
            {
                currentShield -= damage;
            }
            else
            {
                float leftoverDamage = damage - currentShield;
                currentShield = 0;
                ApplyHealthDamage(leftoverDamage);
            }
        }
        else
        {
            ApplyHealthDamage(damage);
        }

        shieldBar.SetShield(currentShield);

        // Start shield recharge
        rechargeCoroutine = StartCoroutine(RechargeShield());
    }

    private IEnumerator RechargeShield()
    {
        // Shield delay
        yield return new WaitForSeconds(rechargeDelay);

        // Shield recharge
        while (currentShield < maxShield)
        {
            currentShield += rechargeRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);

            shieldBar.SetShield((int)currentShield);

            yield return null;
        }
    }

    public void ApplyHealthDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("YOU DIED");
    }

}
