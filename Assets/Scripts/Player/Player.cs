using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    // Experience / level
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    // Level / XP definition class
    public Slider xpSlider;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;

    [System.Serializable]

    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    public List<LevelRange> levelRanges;

    // Gun
    public Gun playerGun;

    // Pause
    public GameObject pauseUI;
    public bool isPaused = false;

    public GameObject HUDUI;

    // Item list
    public List<ItemList> items = new List<ItemList>();

    void Start()
    {
        // Set XP / Level
        experienceCap = levelRanges[0].experienceCapIncrease;


        // Set health / shield
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentShield = maxShield;
        shieldBar.SetMaxShield(maxShield);

        // Set counters
        healthCounter.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        shieldCounter.text = currentShield.ToString() + "/" + maxShield.ToString();

        UpdateUI();
        UpdateXPUI();

        // Start item loop
        StartCoroutine(CallItemUpdate());
    }

    void Update()
    {
        // Damage testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(10);
        }

        // Pause menu
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

    // XP systems
    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();
        UpdateXPUI();
    }

    void LevelUpChecker()
    {
        while (experience >= experienceCap)
        {
            experience -= experienceCap; // XP rollover
            level++;

            int experienceCapIncrease = 0;

            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }

            experienceCap += experienceCapIncrease;

            // Level up effects 
            Debug.Log($"Leveled up! New level: {level}");

            // Trigger level-up UI
            if (LevelUpUI.Instance != null)
            {
                LevelUpUI.Instance.Show(this);
            }
        }
    }

    private void UpdateXPUI()
    {
        if (xpSlider != null)
            xpSlider.value = (float)experience / experienceCap;

        if (xpText != null)
            xpText.text = $"{experience} / {experienceCap} XP";

        if (levelText != null)
            levelText.text = $"Level {level}";
    }

    // Items
    public void AddItem(ItemData data)
    {
        ItemList existing = items.Find(i => i.name == data.itemName);

        if (existing != null)
        {
            existing.stacks++;
            existing.item.OnStacksChanged(this, existing.stacks);
            return;
        }

        Items newItem = data.itemType switch
        {
            ItemType.MaxHealth => new MaxHealth(),
            ItemType.HealthRegen => new HealthRegen(),
            ItemType.MaxShield => new MaxShield(),
            ItemType.ShieldRegen => new ShieldRegen(),
            ItemType.DamageUp => new DamageUp(),
            ItemType.BurnDamage => new BurnDamage(),
            ItemType.FireRateUp => new FireRateUp(),
            ItemType.KnockBackUp => new KnockBackUp(),
            _ => null
        };

        if (newItem != null)
            items.Add(new ItemList(newItem, data.itemName, 1));
    }

    IEnumerator CallItemUpdate()
    {
        // Stop updating items if dead
        if (currentHealth <= 0)
            yield break;

        // Update all items
        foreach (ItemList i in items)
        {
            // Player stat items
            i.item.Update(this, i.stacks);

            // Gun stat items
            if (playerGun != null)
            {
                i.item.Update(playerGun, i.stacks);
            }
        }

        UpdateUI();

        yield return new WaitForSeconds(1);

        StartCoroutine(CallItemUpdate());
    }

    public void CallItemOnHit(Target target)
    {
        foreach (ItemList i in items)
        {
            i.item.OnHit(this, target, i.stacks);
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
            // Stop if dead
            if (currentHealth <= 0)
                yield break;

            currentShield += rechargeRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);

            UpdateUI();

            yield return null;
        }
    }

    // Player damage recieved
    public void ApplyHealthDamage(float amount)
    {
        // If dead
        if (currentHealth <= 0)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Healing
    public void Heal(float amount)
    {
        // Don't heal if the player is dead
        if (currentHealth <= 0)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
    }

    // Shield
    public void RechargeShield(float amount)
    {
        // Stop shield regen if player is dead
        if (currentHealth <= 0)
            return;

        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update sliders
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        shieldBar.SetMaxShield(maxShield);
        shieldBar.SetShield(currentShield);

        // Update text counters
        healthCounter.text = $"{Mathf.CeilToInt(currentHealth)} / {maxHealth}";
        shieldCounter.text = $"{Mathf.CeilToInt(currentShield)} / {maxShield}";
    }

    private void Die()
    {
        Debug.Log("YOU DIED");
    }

}
