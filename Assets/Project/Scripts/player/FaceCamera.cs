using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
            return;

        transform.rotation = mainCamera.transform.rotation;
    }
}