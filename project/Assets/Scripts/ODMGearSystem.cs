using UnityEngine;

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

    [Header("Camera Zoom")]
    public bool dynamicZoom = true;
    public float normalZoom = 5f;
    public float maxZoom = 8f;
    public float zoomSmooth = 3f;

    [Header("Debug")]
    public bool showDebug = false;

    private Camera cam;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement2D>();
        grapplingHook = GetComponent<GrapplingHook>();
        hookVisual = GetComponent<HookVisual>();
        gasBooster = GetComponent<GasBooster>();
        cam = Camera.main;
    }

    private void Start()
    {
        // Auto-setup
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

    private void LateUpdate()
    {
        if (!dynamicZoom || cam == null) return;

        float speed = playerMovement.rb.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(5f, 25f, speed);
        float target = Mathf.Lerp(normalZoom, maxZoom, t);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, target, zoomSmooth * Time.deltaTime);
    }

    private void OnGUI()
    {
        if (!showDebug) return;
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUI.color = Color.white;
        GUILayout.Label($"State: {grapplingHook.currentState}");
        GUILayout.Label($"Hooked: {playerMovement.isHooked}");
        GUILayout.Label($"Grounded: {playerMovement.isGrounded}");
        GUILayout.Label($"Velocity: {playerMovement.rb.linearVelocity}");
        GUILayout.Label($"Speed: {playerMovement.rb.linearVelocity.magnitude:F1}");
        GUILayout.Label($"Gas: {gasBooster.currentGas:F0}/{gasBooster.maxGas}");
        if (grapplingHook.currentState == GrapplingHook.HookState.Attached)
            GUILayout.Label($"Rope: {grapplingHook.currentRopeLength:F1}");
        GUILayout.EndArea();
    }
}