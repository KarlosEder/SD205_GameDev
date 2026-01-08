using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Health 
    public float maxHealth = 100f;
    public float currentHealth;

    // Shield
    public float maxShield = 100f;
    public float currentShield;

    // Shield recharge
    public float rechargeDelay = 3f;
    public float rechargeRate = 40f;

    private Coroutine rechargeCoroutine;

    // Health / shield counters
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI shieldCounter;

    public HealthBar healthBar;
    public HealthBar shieldBar;

    // Pause
    public GameObject pauseUI;
    public bool isPaused = false;

    public GameObject HUDUI;

    void Start()
    {
        // Set health / shield
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentShield = maxShield;
        shieldBar.SetMaxShield(maxShield);

        // Set counters
        healthCounter.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        shieldCounter.text = currentShield.ToString() + "/" + maxShield.ToString();

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Pause game
    void PauseGame()
    {
        pauseUI.SetActive(true);
        HUDUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ResumeGame()
    {
        pauseUI.SetActive(false);
        HUDUI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
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

        /*shieldBar.SetShield(currentShield);*/

        UpdateUI();

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

            /*shieldBar.SetShield((int)currentShield);*/

            UpdateUI();

            yield return null;
        }
    }

    public void ApplyHealthDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        /*healthBar.SetHealth(currentHealth);*/

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        // Update sliders
        healthBar.SetHealth(currentHealth);
        shieldBar.SetHealth(currentShield);

        // Update text counters
        healthCounter.text = $"{Mathf.CeilToInt(currentHealth)} / {maxHealth}";
        shieldCounter.text = $"{Mathf.CeilToInt(currentShield)} / {maxShield}";
    }

    private void Die()
    {
        Debug.Log("YOU DIED");
    }

}
