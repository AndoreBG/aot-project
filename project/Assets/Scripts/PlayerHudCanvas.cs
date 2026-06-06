using UnityEngine;
using UnityEngine.UI;

public static class PlayerHudCanvas
{
    private const string CanvasName = "Player HUD Canvas";

    public static Canvas GetOrCreate()
    {
        GameObject canvasObject = GameObject.Find(CanvasName);
        if (canvasObject == null)
            canvasObject = new GameObject(CanvasName);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        if (canvas == null)
            canvas = canvasObject.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = canvasObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        if (canvasObject.GetComponent<GraphicRaycaster>() == null)
            canvasObject.AddComponent<GraphicRaycaster>();

        return canvas;
    }
}
