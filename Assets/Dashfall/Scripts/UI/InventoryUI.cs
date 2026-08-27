using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotsParent;

    [Header("Slots")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount = 20;

    private PlayerInputActions inputActions;
    private readonly List<InventorySlotUI> slots =
        new List<InventorySlotUI>();

    private bool isOpen;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        CreateSlots();
    }

    private void Start()
    {
        // Fallback: se a referencia nao foi ligada no editor,
        // procura o PlayerInventory automaticamente.
        if (playerInventory == null)
            playerInventory =
                FindAnyObjectByType<PlayerInventory>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.ToggleInventory.performed +=
            OnToggleInventory;
    }

    private void OnDisable()
    {
        inputActions.Player.ToggleInventory.performed -=
            OnToggleInventory;
        inputActions.Disable();
    }

    private void OnToggleInventory(
        UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        SetOpen(!isOpen);
    }

    private void SetOpen(bool open)
    {
        isOpen = open;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(open);

        if (open)
            Refresh();
    }

    private void CreateSlots()
    {
        if (slotsParent == null)
            return;

        // Se os slots ja foram criados no editor
        // (via builder), apenas coleta-os.
        slots.Clear();
        slotsParent.GetComponentsInChildren(slots);

        // Caso contrario, instancia a partir do prefab.
        if (slots.Count == 0 && slotPrefab != null)
        {
            for (int i = 0; i < slotCount; i++)
            {
                GameObject slotObject =
                    Instantiate(slotPrefab, slotsParent);

                InventorySlotUI slot =
                    slotObject.GetComponent<InventorySlotUI>();

                if (slot != null)
                    slots.Add(slot);
            }
        }
    }

    public void Refresh()
    {
        if (playerInventory == null)
            return;

        IReadOnlyList<ItemStack> items =
            playerInventory.Container.Items;

        for (int i = 0; i < slots.Count; i++)
        {
            ItemStack stack =
                (i < items.Count) ? items[i] : null;

            slots[i].SetItem(stack);
        }
    }
}
