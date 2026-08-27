using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text quantityText;

    public void SetItem(ItemStack stack)
    {
        if (stack == null || stack.IsEmpty)
        {
            icon.enabled = false;

            if (quantityText != null)
                quantityText.enabled = false;

            return;
        }

        icon.enabled = true;
        icon.sprite = stack.Item.Icon;

        // Mostra a quantidade apenas quando há empilhamento.
        bool showQuantity = stack.Quantity > 1;

        if (quantityText != null)
        {
            quantityText.enabled = showQuantity;
            quantityText.text = stack.Quantity.ToString();
        }
    }
}
