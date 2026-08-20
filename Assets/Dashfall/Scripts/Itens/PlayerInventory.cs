using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventario")]
    [SerializeField]
    private ItemContainer container = new ItemContainer(20);

    [Header("Teste temporario")]
    [SerializeField] private ItemData startingItem;

    [Min(1)]
    [SerializeField] private int startingQuantity = 1;

    public ItemContainer Container => container;

    private void Awake()
    {
        if (startingItem == null)
            return;

        int remainingQuantity =
            container.AddItem(startingItem, startingQuantity);

        int addedQuantity =
            startingQuantity - remainingQuantity;

        Debug.Log(
            addedQuantity + "x " +
            startingItem.DisplayName +
            " adicionado ao inventario."
        );

        if (remainingQuantity > 0)
        {
            Debug.LogWarning(
                "Nao havia espaco para " +
                remainingQuantity +
                " item(ns)."
            );
        }
    }
}