using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class InventoryUIBuilder
{
    private const string PrefabDir =
        "Assets/Dashfall/Prefabs/UI";

    [MenuItem("Dashfall/UI/Build Inventory UI")]
    public static void Build()
    {
        BuildSlotPrefab();
        BuildInventoryCanvas();
    }

    private static void BuildSlotPrefab()
    {
        EnsureDirectory(PrefabDir);

        string prefabPath =
            PrefabDir + "/InventorySlot.prefab";

        GameObject slot = new GameObject("InventorySlot");

        RectTransform slotRect =
            slot.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(80f, 80f);

        Image slotBackground =
            slot.AddComponent<Image>();
        slotBackground.color = new Color(0f, 0f, 0f, 0.6f);

        // Ícone do item.
        GameObject iconObject =
            new GameObject("Icon", typeof(RectTransform),
                typeof(Image));

        RectTransform iconRect =
            iconObject.GetComponent<RectTransform>();

        iconObject.transform.SetParent(slot.transform, false);
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = new Vector2(5f, 5f);
        iconRect.offsetMax = new Vector2(-5f, -5f);

        // Contador de quantidade.
        GameObject quantityObject =
            new GameObject("Quantity", typeof(RectTransform),
                typeof(Text));

        RectTransform quantityRect =
            quantityObject.GetComponent<RectTransform>();

        quantityObject.transform.SetParent(slot.transform, false);

        quantityRect.anchorMin = Vector2.one;
        quantityRect.anchorMax = Vector2.one;
        quantityRect.pivot = Vector2.one;
        quantityRect.anchoredPosition = new Vector2(-4f, -4f);

        Text quantityText =
            quantityObject.GetComponent<Text>();
        quantityText.font = Resources.GetBuiltinResource<Font>(
            "LegacyRuntime.ttf");
        quantityText.fontSize = 16;
        quantityText.alignment = TextAnchor.MiddleRight;
        quantityText.color = Color.white;
        quantityText.raycastTarget = false;

        InventorySlotUI slotUI =
            slot.AddComponent<InventorySlotUI>();

        SerializedObject serializedSlot =
            new SerializedObject(slotUI);

        serializedSlot.FindProperty("icon").objectReferenceValue =
            iconObject.GetComponent<Image>();
        serializedSlot.FindProperty("quantityText")
            .objectReferenceValue = quantityText;
        serializedSlot.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(slot, prefabPath);

        Object.DestroyImmediate(slot);

        Debug.Log("Prefab de slot criado em: " + prefabPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void BuildInventoryCanvas()
    {
        InventoryUI existing =
            Object.FindObjectOfType<InventoryUI>();

        if (existing != null)
        {
            Debug.LogWarning(
                "InventoryUI ja existe na cena. " +
                "Nada foi criado.");
            return;
        }

        // Canvas Screen Space - Overlay.
        GameObject canvasObject = new GameObject(
            "InventoryCanvas",
            typeof(Canvas), typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas =
            canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        // Painel de inventário.
        GameObject panelObject =
            new GameObject("InventoryPanel",
                typeof(RectTransform),
                typeof(Image), typeof(GridLayoutGroup));

        panelObject.transform.SetParent(
            canvasObject.transform, false);

        RectTransform panelRect =
            panelObject.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(430f, 400f);

        Image panelImage =
            panelObject.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);

        GridLayoutGroup grid =
            panelObject.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(80f, 80f);
        grid.spacing = new Vector2(6f, 6f);
        grid.padding = new RectOffset(5, 5, 5, 5);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        grid.childAlignment = TextAnchor.MiddleCenter;

        // Ajusta o tamanho do painel para caber a grade.
        panelRect.sizeDelta = new Vector2(
            5 + (5 * 80) + (4 * 6) + 5,   // colunas
            5 + (4 * 80) + (3 * 6) + 5);  // linhas (20 slots / 5 = 4)

        string slotPrefabPath =
            PrefabDir + "/InventorySlot.prefab";

        GameObject slotPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(
                slotPrefabPath);

        // Cria os 20 slots no painel.
        for (int i = 0; i < 20; i++)
        {
            Object.Instantiate(slotPrefab, panelObject.transform);
        }

        // Adiciona o controlador da UI.
        InventoryUI inventoryUI =
            canvasObject.AddComponent<InventoryUI>();

        SerializedObject serializedUI =
            new SerializedObject(inventoryUI);

        PlayerInventory playerInventory =
            Object.FindAnyObjectByType<PlayerInventory>();

        serializedUI.FindProperty("playerInventory")
            .objectReferenceValue = playerInventory;
        serializedUI.FindProperty("inventoryPanel")
            .objectReferenceValue = panelObject;
        serializedUI.FindProperty("slotsParent")
            .objectReferenceValue = panelObject.transform as Object;
        serializedUI.FindProperty("slotPrefab")
            .objectReferenceValue = slotPrefab;
        serializedUI.FindProperty("slotCount").intValue = 20;
        serializedUI.ApplyModifiedProperties();

        // Começa fechado.
        panelObject.SetActive(false);

        EditorSceneManager.MarkSceneDirty(
            canvasObject.scene);

        Debug.Log(
            "UI de inventario criada e conectada ao Player.");
    }

    private static void EnsureDirectory(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent =
            Path.GetDirectoryName(path).Replace('\\', '/');
        string folderName =
            Path.GetFileName(path);

        EnsureDirectory(parent);
        AssetDatabase.CreateFolder(parent, folderName);
    }
}
