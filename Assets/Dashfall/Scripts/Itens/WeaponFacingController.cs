using UnityEngine;

public class WeaponFacingController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private SpriteRenderer weaponRenderer;
    [SerializeField] private Transform weaponHolder;

    [Header("Posicao da mao")]
    [SerializeField] private float holderPositionX = 0.3f;

    private void LateUpdate()
    {
        if (characterRenderer == null ||
            weaponRenderer == null ||
            weaponHolder == null)
        {
            return;
        }

        bool lookingLeft = characterRenderer.flipX;

        // Vira a imagem da arma.
        weaponRenderer.flipX = lookingLeft;

        // Move o ponto da arma para a outra mão/lado.
        Vector3 holderPosition = weaponHolder.localPosition;

        holderPosition.x = lookingLeft
            ? -Mathf.Abs(holderPositionX)
            : Mathf.Abs(holderPositionX);

        weaponHolder.localPosition = holderPosition;
    }
}