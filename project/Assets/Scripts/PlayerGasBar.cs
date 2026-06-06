using UnityEngine;
using UnityEngine.UI;

public class PlayerGasBar : MonoBehaviour
{
    [Header("References")]
    public GasBooster gasBooster;
    public Image fillImage;
    public Slider slider;

    [Header("Options")]
    public bool findPlayerOnStart = true;
    public bool createCanvasIfMissing = true;
    public Vector2 anchoredPosition = new Vector2(24f, -54f);
    public Vector2 size = new Vector2(260f, 16f);
    public Color backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.8f);
    public Color fillColor = new Color(0.15f, 0.72f, 0.95f, 1f);

    private RectTransform fillRect;

    private void Awake()
    {
        if (gasBooster == null)
            gasBooster = GetComponent<GasBooster>();

        if (gasBooster == null && findPlayerOnStart)
            gasBooster = FindFirstObjectByType<GasBooster>();
    }

    private void Start()
    {
        if (gasBooster == null)
            return;

        if (fillImage == null && slider == null && createCanvasIfMissing)
            CreateRuntimeBar();

        gasBooster.GasChanged += UpdateBar;
        UpdateBar(gasBooster.currentGas, gasBooster.maxGas);
    }

    private void LateUpdate()
    {
        if (gasBooster != null)
            UpdateBar(gasBooster.currentGas, gasBooster.maxGas);
    }

    private void OnDestroy()
    {
        if (gasBooster != null)
            gasBooster.GasChanged -= UpdateBar;
    }

    private void UpdateBar(float currentGas, float maxGas)
    {
        float percent = maxGas <= 0f ? 0f : currentGas / maxGas;

        if (fillImage != null)
            fillImage.fillAmount = percent;

        if (fillRect != null)
            fillRect.anchorMax = new Vector2(percent, 1f);

        if (slider != null)
        {
            slider.maxValue = maxGas;
            slider.value = currentGas;
        }
    }

    private void CreateRuntimeBar()
    {
        Canvas canvas = PlayerHudCanvas.GetOrCreate();

        GameObject backgroundObject = new GameObject("Gas Background");
        backgroundObject.transform.SetParent(canvas.transform, false);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = backgroundColor;

        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 1f);
        backgroundRect.anchorMax = new Vector2(0f, 1f);
        backgroundRect.pivot = new Vector2(0f, 1f);
        backgroundRect.anchoredPosition = anchoredPosition;
        backgroundRect.sizeDelta = size;

        GameObject fillObject = new GameObject("Gas Fill");
        fillObject.transform.SetParent(backgroundObject.transform, false);
        fillImage = fillObject.AddComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;

        fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(3f, 3f);
        fillRect.offsetMax = new Vector2(-3f, -3f);
    }
}
