using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Image fillImage;
    public Slider slider;

    [Header("Options")]
    public bool findPlayerOnStart = true;
    public bool createCanvasIfMissing = true;
    public Vector2 anchoredPosition = new Vector2(24f, -24f);
    public Vector2 size = new Vector2(260f, 22f);
    public Color backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.8f);
    public Color fillColor = new Color(0.76f, 0.08f, 0.08f, 1f);

    private RectTransform fillRect;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null && findPlayerOnStart)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void Start()
    {
        if (playerHealth == null)
            return;

        if (fillImage == null && slider == null && createCanvasIfMissing)
            CreateRuntimeBar();

        playerHealth.HealthChanged += UpdateBar;
        UpdateBar(playerHealth.currentHealth, playerHealth.maxHealth);
    }

    private void LateUpdate()
    {
        if (playerHealth != null)
            UpdateBar(playerHealth.currentHealth, playerHealth.maxHealth);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.HealthChanged -= UpdateBar;
    }

    private void UpdateBar(float currentHealth, float maxHealth)
    {
        float percent = maxHealth <= 0f ? 0f : currentHealth / maxHealth;

        if (fillImage != null)
            fillImage.fillAmount = percent;

        if (fillRect != null)
            fillRect.anchorMax = new Vector2(percent, 1f);

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }

    private void CreateRuntimeBar()
    {
        Canvas canvas = PlayerHudCanvas.GetOrCreate();

        GameObject backgroundObject = new GameObject("Health Background");
        backgroundObject.transform.SetParent(canvas.transform, false);
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = backgroundColor;

        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 1f);
        backgroundRect.anchorMax = new Vector2(0f, 1f);
        backgroundRect.pivot = new Vector2(0f, 1f);
        backgroundRect.anchoredPosition = anchoredPosition;
        backgroundRect.sizeDelta = size;

        GameObject fillObject = new GameObject("Health Fill");
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
