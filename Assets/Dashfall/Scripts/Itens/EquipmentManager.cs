using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private SpriteRenderer weaponRenderer;

    [Header("Equipamento inicial para teste")]
    [SerializeField] private ItemData startingWeapon;

    public ItemData EquippedWeapon { get; private set; }

    private void Awake()
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.sprite = null;
            weaponRenderer.enabled = false;
        }
    }

    private void Start()
    {
        if (startingWeapon != null)
        {
            EquipWeapon(startingWeapon);
        }
    }

    public bool EquipWeapon(ItemData weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Nenhuma arma foi informada.");
            return false;
        }

        if (weapon.Type != ItemType.Weapon)
        {
            Debug.LogWarning(
                weapon.DisplayName + " nao e uma arma."
            );

            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError(
                "PlayerInventory nao foi configurado."
            );

            return false;
        }

        if (!playerInventory.Container.HasItem(weapon))
        {
            Debug.LogWarning(
                weapon.DisplayName +
                " nao esta no inventario."
            );

            return false;
        }

        if (weaponRenderer == null)
        {
            Debug.LogError(
                "Weapon Renderer nao foi configurado."
            );

            return false;
        }

        if (weapon.EquippedSprite == null)
        {
            Debug.LogWarning(
                weapon.DisplayName +
                " nao possui Equipped Sprite."
            );

            return false;
        }

        EquippedWeapon = weapon;

        weaponRenderer.sprite = weapon.EquippedSprite;
        weaponRenderer.enabled = true;

        Debug.Log(
            weapon.DisplayName + " foi equipada."
        );

        return true;
    }

    public void UnequipWeapon()
    {
        EquippedWeapon = null;

        if (weaponRenderer != null)
        {
            weaponRenderer.sprite = null;
            weaponRenderer.enabled = false;
        }

        Debug.Log("Arma desequipada.");
    }
}