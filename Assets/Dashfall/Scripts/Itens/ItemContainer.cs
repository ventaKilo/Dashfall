using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemContainer
{
    [Min(1)]
    [SerializeField] private int capacity = 20;

    [SerializeField] private List<ItemStack> items = new List<ItemStack>();

    public int Capacity => capacity;
    public IReadOnlyList<ItemStack> Items => items;

    public ItemContainer(int capacity)
    {
        this.capacity = capacity;
    }

    // Retorna a quantidade que não coube.
    public int AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return quantity;

        int remainingQuantity = quantity;

        // Primeiro tenta completar pilhas existentes.
        foreach (ItemStack stack in items)
        {
            if (stack.CanStackWith(item))
            {
                remainingQuantity =
                    stack.AddQuantity(remainingQuantity);

                if (remainingQuantity <= 0)
                    return 0;
            }
        }

        // Cria novas pilhas enquanto existirem espaços.
        while (remainingQuantity > 0 &&
               items.Count < capacity)
        {
            int quantityForNewStack =
                Mathf.Min(remainingQuantity, item.MaxStack);

            items.Add(
                new ItemStack(item, quantityForNewStack)
            );

            remainingQuantity -= quantityForNewStack;
        }

        return remainingQuantity;
    }

    public bool HasItem(ItemData item, int quantity = 1)
    {
        int totalQuantity = 0;

        foreach (ItemStack stack in items)
        {
            if (stack.Item == item)
            {
                totalQuantity += stack.Quantity;

                if (totalQuantity >= quantity)
                    return true;
            }
        }

        return false;
    }

    // Retorna a quantidade realmente removida.
    public int RemoveItem(ItemData item, int quantity = 1)
    {
        int remainingToRemove = quantity;
        int totalRemoved = 0;

        for (int i = items.Count - 1;
             i >= 0 && remainingToRemove > 0;
             i--)
        {
            ItemStack stack = items[i];

            if (stack.Item != item)
                continue;

            int removed =
                stack.RemoveQuantity(remainingToRemove);

            totalRemoved += removed;
            remainingToRemove -= removed;

            if (stack.IsEmpty)
                items.RemoveAt(i);
        }

        return totalRemoved;
    }
}