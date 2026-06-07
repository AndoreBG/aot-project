using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDecorativeHud : MonoBehaviour
{
    private const string RootName = "Decorative HUD";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateOnSceneLoad()
    {
        if (SceneManager.GetActiveScene().name == "Menu" || FindFirstObjectByType<PlayerHealth>() == null)
            return;

        Canvas canvas = PlayerHudCanvas.GetOrCreate();
        if (canvas.transform.Find(RootName) != null)
            return;

        GameObject hudObject = new GameObject(RootName, typeof(RectTransform));
        hudObject.transform.SetParent(canvas.transform, false);
        PlayerDecorativeHud hud = hudObject.AddComponent<PlayerDecorativeHud>();
        hud.Build();
    }

    private void Build()
    {
        RectTransform root = GetComponent<RectTransform>();
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        CreateUtilityInventory(root);
        CreateMinimap(root);
    }

    private void CreateUtilityInventory(RectTransform root)
    {
        RectTransform panel = CreateRect("Utility Inventory", root, new Vector2(0f, 0f), new Vector2(0f, 0f),
            new Vector2(0f, 0f), new Vector2(44f, 42f), new Vector2(178f, 178f));

        CreateSlot(panel, "Main Slot", new Vector2(0f, 0f), new Vector2(76f, 76f), true, new Color(0.62f, 0.55f, 0.44f, 0.95f));
        CreateSlot(panel, "Top Slot", new Vector2(0f, 54f), new Vector2(52f, 52f), false, new Color(0.20f, 0.22f, 0.24f, 0.9f));
        CreateSlot(panel, "Left Slot", new Vector2(-54f, 0f), new Vector2(52f, 52f), false, new Color(0.12f, 0.14f, 0.15f, 0.88f));
        CreateSlot(panel, "Right Slot", new Vector2(54f, 0f), new Vector2(52f, 52f), false, new Color(0.12f, 0.14f, 0.15f, 0.88f));
        CreateSlot(panel, "Bottom Slot", new Vector2(0f, -54f), new Vector2(52f, 52f), false, new Color(0.12f, 0.14f, 0.15f, 0.88f));

        CreateImage("Quick Item Glow", panel, new Vector2(0f, 0f), new Vector2(58f, 58f), new Color(0.95f, 0.8f, 0.38f, 0.22f));
        CreateImage("Flask Shape", panel, new Vector2(0f, -2f), new Vector2(24f, 42f), new Color(0.93f, 0.56f, 0.18f, 0.86f));
        CreateImage("Flask Neck", panel, new Vector2(0f, 24f), new Vector2(13f, 13f), new Color(0.95f, 0.75f, 0.32f, 0.9f));
        CreateImage("Top Utility Mark", panel, new Vector2(0f, 54f), new Vector2(18f, 24f), new Color(0.72f, 0.76f, 0.78f, 0.58f));
        CreateImage("Left Utility Mark", panel, new Vector2(-54f, 0f), new Vector2(22f, 12f), new Color(0.72f, 0.76f, 0.78f, 0.46f));
        CreateImage("Right Utility Mark", panel, new Vector2(54f, 0f), new Vector2(18f, 18f), new Color(0.72f, 0.76f, 0.78f, 0.46f));
        CreateImage("Bottom Utility Mark", panel, new Vector2(0f, -54f), new Vector2(26f, 8f), new Color(0.72f, 0.76f, 0.78f, 0.42f));
    }

    private void CreateMinimap(RectTransform root)
    {
        RectTransform panel = CreateRect("Minimap Container", root, new Vector2(1f, 0f), new Vector2(1f, 0f),
            new Vector2(1f, 0f), new Vector2(-42f, 42f), new Vector2(250f, 164f));

        Image frame = panel.gameObject.AddComponent<Image>();
        frame.color = new Color(0.02f, 0.025f, 0.03f, 0.76f);
        frame.raycastTarget = false;

        CreateImage("Map Field", panel, Vector2.zero, new Vector2(228f, 142f), new Color(0.055f, 0.075f, 0.07f, 0.86f));
        CreateImage("Map North Ridge", panel, new Vector2(-22f, 42f), new Vector2(154f, 10f), new Color(0.22f, 0.30f, 0.26f, 0.72f));
        CreateImage("Map South Ridge", panel, new Vector2(26f, -44f), new Vector2(176f, 9f), new Color(0.20f, 0.28f, 0.24f, 0.65f));
        CreateImage("Map Path Horizontal", panel, new Vector2(-12f, 2f), new Vector2(166f, 7f), new Color(0.48f, 0.45f, 0.34f, 0.58f));
        CreateImage("Map Path Vertical", panel, new Vector2(44f, 4f), new Vector2(7f, 100f), new Color(0.48f, 0.45f, 0.34f, 0.5f));
        CreateImage("Map Room A", panel, new Vector2(-76f, 24f), new Vector2(34f, 26f), new Color(0.13f, 0.18f, 0.17f, 0.88f));
        CreateImage("Map Room B", panel, new Vector2(72f, -24f), new Vector2(44f, 34f), new Color(0.13f, 0.18f, 0.17f, 0.88f));
        CreateImage("Player Dot", panel, new Vector2(18f, -2f), new Vector2(12f, 12f), new Color(0.95f, 0.18f, 0.14f, 0.95f));
        CreateImage("Minimap Shine", panel, new Vector2(-70f, 52f), new Vector2(88f, 4f), new Color(0.95f, 0.95f, 0.84f, 0.18f));
    }

    private void CreateSlot(RectTransform parent, string name, Vector2 position, Vector2 size, bool selected, Color fillColor)
    {
        RectTransform slot = CreateRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), position, size);

        Image background = slot.gameObject.AddComponent<Image>();
        background.color = fillColor;
        background.raycastTarget = false;

        CreateImage(name + " Inner Shadow", slot, Vector2.zero, size - new Vector2(10f, 10f), new Color(0f, 0f, 0f, selected ? 0.28f : 0.42f));

        if (selected)
            CreateImage(name + " Amber Edge", slot, Vector2.zero, size - new Vector2(5f, 5f), new Color(0.94f, 0.72f, 0.34f, 0.16f));
    }

    private Image CreateImage(string name, RectTransform parent, Vector2 position, Vector2 size, Color color)
    {
        RectTransform rect = CreateRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), position, size);

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private RectTransform CreateRect(string name, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }
}
