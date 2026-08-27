using System;
using UnityEngine;

[Serializable]
public class ItemStack
{
    [SerializeField] private ItemData item;

    [Min(1)]
    [SerializeField] private int quantity = 1;

    [Min(0)]
    [SerializeField] private int currentDurability;

    public ItemData Item => item;
    public int Quantity => quantity;
    public int CurrentDurability => currentDurability;

    public bool IsEmpty => item == null || quantity <= 0;

    public ItemStack(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = Mathf.Clamp(quantity, 1, item.MaxStack);
        currentDurability = item.MaxDurability;
    }

    public bool CanStackWith(ItemData otherItem)
    {
        return item == otherItem &&
               quantity < item.MaxStack;
    }

    // Retorna a quantidade que não coube.
    public int AddQuantity(int amount)
    {
        if (amount <= 0 || item == null)
            return amount;

        int availableSpace = item.MaxStack - quantity;
        int amountAdded = Mathf.Min(availableSpace, amount);

        quantity += amountAdded;

        return amount - amountAdded;
    }

    // Retorna a quantidade removida.
    public int RemoveQuantity(int amount)
    {
        int amountRemoved = Mathf.Min(quantity, amount);

        quantity -= amountRemoved;

        return amountRemoved;
    }
}