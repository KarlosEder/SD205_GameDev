using UnityEngine;

// All items
public enum ItemType
{
    MaxHealth,
    HealthRegen,
    MaxShield,
    ShieldRegen,
    DamageUp,
    BurnDamage,
    FireRateUp,
    KnockBackUp
}

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
}