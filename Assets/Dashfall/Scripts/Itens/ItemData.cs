using UnityEngine;

public enum ItemType
{
    Resource,
    Consumable,
    Weapon,
    Armor,
    Tool
}

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "Dashfall/Items/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Informacoes basicas")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [TextArea]
    [SerializeField] private string description;

    [Header("Tipo e quantidade")]
    [SerializeField] private ItemType itemType;
    [Min(1)]
    [SerializeField] private int maxStack = 1;

    [Header("Visual")]
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite equippedSprite;

    [Header("Configuracoes de arma")]
    [Min(0)]
    [SerializeField] private int baseDamage;

    [Min(0)]
    [SerializeField] private int maxDurability;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;
    public ItemType Type => itemType;
    public int MaxStack => maxStack;
    public Sprite Icon => icon;
    public Sprite EquippedSprite => equippedSprite;
    public int BaseDamage => baseDamage;
    public int MaxDurability => maxDurability;
}