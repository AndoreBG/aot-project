using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using Unity.Cinemachine;
#else
using Cinemachine;
#endif

[RequireComponent(typeof(PlayerMovement2D))]
[RequireComponent(typeof(GrapplingHook))]
[RequireComponent(typeof(HookVisual))]
[RequireComponent(typeof(GasBooster))]
public class ODMGearSystem : MonoBehaviour
{
    public PlayerMovement2D playerMovement;
    public GrapplingHook grapplingHook;
    public HookVisual hookVisual;
    public GasBooster gasBooster;

    [Header("Cinemachine")]
    [Tooltip("Arraste a CinemachineCamera da cena aqui. Se vazio, tenta encontrar automaticamente.")]
    public CinemachineCamera cinemachineCamera;

    [Header("Dynamic Zoom")]
    public bool dynamicZoom = true;
    public float normalZoom = 5f;
    public float maxZoom = 8f;
    public float zoomSmooth = 3f;
    public float zoomSpeedThresholdMin = 5f;
    public float zoomSpeedThresholdMax = 25f;

    [Header("Screen Shake")]
    public bool enableShake = true;
    public float hookAttachShakeIntensity = 1.5f;
    public float hookAttachShakeDuration = 0.15f;
    public float burstShakeIntensity = 0.8f;
    public float burstShakeDuration = 0.1f;
    public float speedShakeIntensity = 0.3f;
    public float speedShakeThreshold = 18f;

    [Header("Debug")]
    public bool showDebug = false;

    // Shake state
    private float shakeTimer;
    private float shakeIntensity;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement2D>();
        grapplingHook = GetComponent<GrapplingHook>();
        hookVisual = GetComponent<HookVisual>();
        gasBooster = GetComponent<GasBooster>();
    }

    private void Start()
    {
        AutoSetup();
        SetupCinemachine();
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void LateUpdate()
    {
        if (cinemachineCamera == null) return;

        UpdateDynamicZoom();
        UpdateShake();
    }

    // ═══════════════════════════════════════════
    // CINEMACHINE SETUP
    // ═══════════════════════════════════════════
    private void SetupCinemachine()
    {
        // Tentar encontrar CinemachineCamera automaticamente
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        if (cinemachineCamera == null)
        {
            Debug.LogWarning("ODM Gear: Nenhuma CinemachineCamera encontrada! " +
                             "Efeitos de câmera desativados. " +
                             "Arraste uma CinemachineCamera no campo do inspector.");
            dynamicZoom = false;
            enableShake = false;
            return;
        }

        // Configurar componente de noise para shake
        noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null && enableShake)
        {
            // Adicionar componente de noise automaticamente
            noise = cinemachineCamera.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();

            // Tentar usar um noise profile padrão
            // Se não encontrar, o shake vai funcionar mas sem profile visual
            // O usuário pode atribuir um manualmente depois
            Debug.Log("ODM Gear: CinemachineBasicMultiChannelPerlin adicionado automaticamente. " +
                      "Para melhor resultado, atribua um Noise Profile (ex: '6D Shake') no inspector.");
        }

        // Inicializar sem shake
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }

        // Garantir zoom inicial
        cinemachineCamera.Lens.OrthographicSize = normalZoom;

        Debug.Log($"ODM Gear: CinemachineCamera '{cinemachineCamera.name}' configurada com sucesso.");
    }

    // ═══════════════════════════════════════════
    // DYNAMIC ZOOM
    // ═══════════════════════════════════════════
    private void UpdateDynamicZoom()
    {
        if (!dynamicZoom) return;

        float speed = playerMovement.rb.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(zoomSpeedThresholdMin, zoomSpeedThresholdMax, speed);
        float targetZoom = Mathf.Lerp(normalZoom, maxZoom, t);

        float currentZoom = cinemachineCamera.Lens.OrthographicSize;
        float newZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSmooth * Time.deltaTime);

        cinemachineCamera.Lens.OrthographicSize = newZoom;
    }

    // ═══════════════════════════════════════════
    // SCREEN SHAKE
    // ═══════════════════════════════════════════
    public void TriggerShake(float intensity, float duration)
    {
        if (!enableShake || noise == null) return;

        // Só sobrescrever se o novo shake é mais forte
        if (intensity > shakeIntensity || shakeTimer <= 0f)
        {
            shakeIntensity = intensity;
            shakeTimer = duration;
        }
    }

    private void UpdateShake()
    {
        if (!enableShake || noise == null) return;

        // Speed shake contínuo (leve tremor em alta velocidade)
        float speed = playerMovement.rb.linearVelocity.magnitude;
        float speedShake = 0f;
        if (speed > speedShakeThreshold)
        {
            float t = Mathf.InverseLerp(speedShakeThreshold, speedShakeThreshold * 2f, speed);
            speedShake = Mathf.Lerp(0f, speedShakeIntensity, t);
        }

        // Event shake (impacto, burst)
        float eventShake = 0f;
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            // Fade out
            float fadeT = Mathf.Clamp01(shakeTimer / 0.1f); // fade nos últimos 0.1s
            eventShake = shakeIntensity * fadeT;

            if (shakeTimer <= 0f)
            {
                shakeIntensity = 0f;
            }
        }

        // Combinar (usar o maior)
        float finalShake = Mathf.Max(speedShake, eventShake);
        noise.AmplitudeGain = finalShake;
        noise.FrequencyGain = finalShake > 0.01f ? 1f : 0f;
    }

    // ═══════════════════════════════════════════
    // EVENTS
    // ═══════════════════════════════════════════
    private void SubscribeEvents()
    {
        if (grapplingHook != null)
        {
            grapplingHook.OnHookAttached += OnHookAttached;
            grapplingHook.OnHookRetractStart += OnHookReleased;
        }
    }

    private void UnsubscribeEvents()
    {
        if (grapplingHook != null)
        {
            grapplingHook.OnHookAttached -= OnHookAttached;
            grapplingHook.OnHookRetractStart -= OnHookReleased;
        }
    }

    private void OnHookAttached()
    {
        TriggerShake(hookAttachShakeIntensity, hookAttachShakeDuration);
    }

    private void OnHookReleased()
    {
        // Shake leve ao soltar
        TriggerShake(hookAttachShakeIntensity * 0.5f, hookAttachShakeDuration * 0.5f);
    }

    // Chamado pelo GasBooster quando faz burst
    public void OnGasBurst()
    {
        TriggerShake(burstShakeIntensity, burstShakeDuration);
    }

    // ═══════════════════════════════════════════
    // AUTO SETUP
    // ═══════════════════════════════════════════
    private void AutoSetup()
    {
        if (grapplingHook.leftAnchor == null)
        {
            var go = new GameObject("AnchorL");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(-0.3f, 0.1f, 0);
            grapplingHook.leftAnchor = go.transform;
        }
        if (grapplingHook.rightAnchor == null)
        {
            var go = new GameObject("AnchorR");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0.3f, 0.1f, 0);
            grapplingHook.rightAnchor = go.transform;
        }
        if (playerMovement.groundCheckPoint == null)
        {
            var go = new GameObject("GroundCheck");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0, -0.5f, 0);
            playerMovement.groundCheckPoint = go.transform;
        }
        if (grapplingHook.grappleableLayers == 0)
            grapplingHook.grappleableLayers = playerMovement.groundLayer;
    }

    // ═══════════════════════════════════════════
    // DEBUG
    // ═══════════════════════════════════════════
    private void OnGUI()
    {
        if (!showDebug) return;
        GUILayout.BeginArea(new Rect(10, 10, 300, 250));
        GUI.color = Color.white;
        GUILayout.Label($"<b>═══ ODM Gear Debug ═══</b>");
        GUILayout.Label($"Hook State: {grapplingHook.currentState}");
        GUILayout.Label($"Hooked: {playerMovement.isHooked}");
        GUILayout.Label($"Grounded: {playerMovement.isGrounded}");
        GUILayout.Label($"Velocity: {playerMovement.rb.linearVelocity:F1}");
        GUILayout.Label($"Speed: {playerMovement.rb.linearVelocity.magnitude:F1}");
        GUILayout.Label($"Gas: {gasBooster.currentGas:F0}/{gasBooster.maxGas}");
        GUILayout.Label($"Intended Dir: {playerMovement.intendedDirection}");
        if (grapplingHook.currentState == GrapplingHook.HookState.Attached)
            GUILayout.Label($"Rope: {grapplingHook.currentRopeLength:F1}");
        if (cinemachineCamera != null)
            GUILayout.Label($"Zoom: {cinemachineCamera.Lens.OrthographicSize:F1}");
        GUILayout.EndArea();
    }
}