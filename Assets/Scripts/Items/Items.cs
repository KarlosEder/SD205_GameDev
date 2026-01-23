using UnityEngine;

[System.Serializable]

public abstract class Items
{
    public abstract string GiveName();

    public virtual void Update(Player player, int stacks)
    {

    }
    public virtual void Update(Gun gun, int stacks)
    {

    }
    public virtual void OnHit(Player player, Target target, int stacks)
    {

    }
    public virtual void OnTick(Player player, int stacks)
    {

    }
    public virtual void OnStacksChanged(Player player, int stacks)
    {

    }
}

// - - - - Healing items - - - -

// Max Health
public class MaxHealth : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Max Health";
    }

    // Health increase amount
    public override void Update(Player player, int stacks)
    {
        if (stacks <= appliedStacks)
            return;

        int newStacks = stacks - appliedStacks;

        int healthPerStack = 10; 

        int totalIncrease = newStacks * healthPerStack;

        player.maxHealth += totalIncrease;
        player.currentHealth += totalIncrease;

        appliedStacks = stacks;
    }
}

// Health Regen
public class HealthRegen : Items
{
    public override string GiveName()
    {
        return "Health Regen";
    }

    // Regen amount
    public override void Update(Player player, int stacks)
    {
        player.Heal(3 + (2 * stacks));
    }
}

// Max Shield
public class MaxShield : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Max Shield";
    }

    // Shield increase amount
    public override void Update(Player player, int stacks)
    {
        if (stacks <= appliedStacks)
            return;

        int newStacks = stacks - appliedStacks;

        int shieldPerStack = 10;

        int totalIncrease = newStacks * shieldPerStack;

        player.maxShield += totalIncrease;
        player.currentShield += totalIncrease;

        appliedStacks = stacks;
    }
}

// Shield Regen
public class ShieldRegen : Items
{
    public override string GiveName()
    {
        return "Shield Regen";
    }

    // Shield regen amount
    public override void Update(Player player, int stacks)
    {
        player.RechargeShield(3 + (2 * stacks));
    }
}

// Shield Recharge Delay
public class ShieldDelay : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Shield Delay";
    }

    // Shield delay amount
    public override void Update(Player player, int stacks)
    {
        if (stacks <= appliedStacks)
            return;

        int newStacks = stacks - appliedStacks;

        float reductionPerStack = 0.2f; 

        // Reduce recharge delay
        player.rechargeDelay -= newStacks * reductionPerStack;

        // Clamp to 0.1
        player.rechargeDelay = Mathf.Max(player.rechargeDelay, 0.1f);

        appliedStacks = stacks;
    }
}

// - - - - Damage items - - - -

// DMG up
public class DamageUp : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Damage Up";
    }

    // Damage increase amount
    public override void Update(Gun gun, int stacks)
    {
        if (stacks <= appliedStacks) return;

        int newStacks = stacks - appliedStacks;
        gun.damage += newStacks * 2f; 
        appliedStacks = stacks;
    }
}

// Burn DOT
public class BurnDamage : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Burn Damage Up";
    }

    // Burn Damage increase amount
    public override void Update(Gun gun, int stacks)
    {
        if (stacks <= appliedStacks) return;

        int newStacks = stacks - appliedStacks;
        gun.burnDamage += newStacks * 2.5f;

        if (gun.burnDuration <= 0f)
            gun.burnDuration = 3f;       
        if (gun.burnTickRate <= 0f)
            gun.burnTickRate = 0.5f;

        appliedStacks = stacks;

        // TEST
        Debug.Log($"Applying BurnDamage item: gun.burnDamage={gun.burnDamage}");
    }
}

// Fire Rate Up
public class FireRateUp : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Fire Rate Up";
    }

    // Fire Rate increase amount
    public override void Update(Gun gun, int stacks)
    {
        if (stacks <= appliedStacks) return;

        int newStacks = stacks - appliedStacks;
        gun.fireRate += newStacks * 2f;
        appliedStacks = stacks;
    }
}

// Knock Back Up
public class KnockBackUp : Items
{
    private int appliedStacks = 0;

    public override string GiveName()
    {
        return "Knock Back Up";
    }

    // Knock back increase amount
    public override void Update(Gun gun, int stacks)
    {
        if (stacks <= appliedStacks) return;

        int newStacks = stacks - appliedStacks;
        gun.knockback += newStacks * 5f;
        appliedStacks = stacks;
    }
}
