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
}

// - - - - Healing items - - - -

// Max Health
public class MaxHealth : Items
{
    public override string GiveName()
    {
        return "Max Health";
    }

    // Health increase amount
    public override void Update(Player player, int stacks)
    {
        player.maxHealth += 3 + (2 * stacks);
    }
}

// Health Regen
public class HealthRegen : Items
{
    public override string GiveName()
    {
        return "Health Regen";
    }

    // Health regen amount
    public override void Update(Player player, int stacks)
    {
        player.currentHealth += 3 + (2* stacks);
    }
}

// Max Shield
public class MaxShield : Items
{
    public override string GiveName()
    {
        return "Max Shield";
    }

    // Shield increase amount
    public override void Update(Player player, int stacks)
    {
        player.maxShield += 3 + (2 * stacks);
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
        player.currentShield += 3 + (2 * stacks);
    }
}

// Shield Recharge Delay
public class ShieldDelay : Items
{
    public override string GiveName()
    {
        return "Shield Delay";
    }

    // Shield delay amount
    public override void Update(Player player, int stacks)
    {
        player.rechargeDelay += 1 + (2 * stacks);
    }
}

// - - - - Damage items - - - -

// DMG up
public class DamageUp : Items
{
    public override string GiveName()
    {
        return "Damage Up";
    }

    public override void OnHit(Player player, Target target, int stacks)
    {
        target.health -= 10 * stacks;
    }
}

    // Burn DOT
    public class BurnDamage : Items
{
    public override string GiveName()
    {
        return "Burn Damage";
    }

    public override void OnHit(Player player, Target target, int stacks)
    {
        target.health -= 10 * stacks;
    }
}

// Fire Rate Up
public class FireRateUp : Items
{
    public override string GiveName()
    {
        return "Fire Rate Up";
    }
    public override void Update(Gun gun, int stacks)
    {
        gun.fireRate += 3 + (2 * stacks);
    }

}

// Knock Back Up
public class KnockBackUp : Items
{
    public override string GiveName()
    {
        return "Knock Back Up";
    }
    public override void Update(Gun gun, int stacks)
    {
        gun.knockback += 13 + (2 * stacks);
    }

}