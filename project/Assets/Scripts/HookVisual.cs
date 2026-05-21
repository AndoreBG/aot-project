using UnityEngine;

[RequireComponent(typeof(GrapplingHook))]
public class HookVisual : MonoBehaviour
{
    [Header("Line Renderers (auto-created if null)")]
    public LineRenderer leftCable;
    public LineRenderer rightCable;

    [Header("Cable Settings")]
    public int segments = 25;
    public float cableWidth = 0.035f;
    public Color cableColor = new Color(0.55f, 0.55f, 0.55f, 1f);

    [Header("Cable Wave (in flight)")]
    public float waveAmplitude = 0.08f;
    public float waveFrequency = 4f;

    [Header("Cable Sag (when attached)")]
    public float sagAmount = 0.4f;

    [Header("Hook Tips")]
    public GameObject hookTipPrefab;
    public float tipSize = 0.25f;

    [Header("Fire Flash")]
    public GameObject fireFlashPrefab;

    private GrapplingHook hook;
    private GameObject tipL, tipR;
    private float wavePhase;

    private void Awake()
    {
        hook = GetComponent<GrapplingHook>();
    }

    private void Start()
    {
        if (leftCable == null) leftCable = MakeLine("CableL");
        if (rightCable == null) rightCable = MakeLine("CableR");
        tipL = MakeTip("TipL");
        tipR = MakeTip("TipR");

        hook.OnHookFired += () =>
        {
            if (fireFlashPrefab != null)
            {
                Destroy(Instantiate(fireFlashPrefab, hook.GetLeftAnchor(), Quaternion.identity), 0.2f);
                Destroy(Instantiate(fireFlashPrefab, hook.GetRightAnchor(), Quaternion.identity), 0.2f);
            }
        };

        SetActive(false);
    }

    private void LateUpdate()
    {
        if (hook.currentState == GrapplingHook.HookState.Idle)
        {
            SetActive(false);
            return;
        }

        SetActive(true);
        wavePhase += Time.deltaTime * waveFrequency * Mathf.PI * 2f;

        Vector2 tipPos = hook.hookTipPosition;
        Vector2 anchorL = hook.GetLeftAnchor();
        Vector2 anchorR = hook.GetRightAnchor();

        // Separação visual das pontas
        float sep = 0.12f;
        Vector2 dirFromCenter = (tipPos - hook.GetAnchorCenter());
        Vector2 perp = Vector2.zero;

        if (dirFromCenter.magnitude > 0.01f)
        {
            Vector2 d = dirFromCenter.normalized;
            perp = new Vector2(-d.y, d.x) * sep;
        }

        Vector2 tipLPos, tipRPos;

        if (hook.currentState == GrapplingHook.HookState.Attached)
        {
            // Ambas pontas convergem no ponto de contato
            tipLPos = tipPos;
            tipRPos = tipPos;
        }
        else
        {
            tipLPos = tipPos - perp;
            tipRPos = tipPos + perp;
        }

        tipL.transform.position = tipLPos;
        tipR.transform.position = tipRPos;

        // Rotação das pontas
        RotateTip(tipL, tipLPos, anchorL);
        RotateTip(tipR, tipRPos, anchorR);

        // Cabos
        DrawCable(leftCable, anchorL, tipLPos);
        DrawCable(rightCable, anchorR, tipRPos);
    }

    private void DrawCable(LineRenderer lr, Vector2 start, Vector2 end)
    {
        lr.positionCount = segments;
        Vector2 delta = end - start;
        float dist = delta.magnitude;
        Vector2 dir = dist > 0.001f ? delta / dist : Vector2.up;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        bool attached = hook.currentState == GrapplingHook.HookState.Attached;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector2 basePos = Vector2.Lerp(start, end, t);

            // Envelope: zero nas pontas, máximo no meio
            float envelope = Mathf.Sin(t * Mathf.PI);
            float offset = 0f;

            if (attached)
            {
                // Catenária (sag para baixo)
                float normalizedDist = dist / hook.maxRange;
                offset = -sagAmount * envelope * normalizedDist;
                // Aplicar para baixo, não perpendicular
                lr.SetPosition(i, basePos + Vector2.down * Mathf.Abs(offset));
            }
            else
            {
                // Ondulação durante voo/retração
                offset = Mathf.Sin(t * waveFrequency * Mathf.PI + wavePhase)
                         * waveAmplitude * envelope;
                lr.SetPosition(i, basePos + perp * offset);
            }
        }
    }

    private void SetActive(bool on)
    {
        if (leftCable != null) leftCable.enabled = on;
        if (rightCable != null) rightCable.enabled = on;
        if (tipL != null) tipL.SetActive(on);
        if (tipR != null) tipR.SetActive(on);
    }

    private void RotateTip(GameObject tip, Vector2 tipPos, Vector2 anchor)
    {
        Vector2 d = (tipPos - anchor);
        if (d.magnitude < 0.01f) return;
        float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        tip.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private LineRenderer MakeLine(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = cableColor;
        lr.endColor = cableColor;
        lr.startWidth = cableWidth;
        lr.endWidth = cableWidth * 0.6f;
        lr.positionCount = segments;
        lr.sortingOrder = 5;
        lr.useWorldSpace = true;
        lr.numCapVertices = 3;
        return lr;
    }

    private GameObject MakeTip(string name)
    {
        if (hookTipPrefab != null)
        {
            GameObject t = Instantiate(hookTipPrefab);
            t.name = name;
            return t;
        }

        GameObject obj = new GameObject(name);
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 6;

        // Triângulo simples como placeholder
        Texture2D tex = new Texture2D(8, 8);
        Color[] px = new Color[64];
        for (int i = 0; i < 64; i++)
        {
            int x = i % 8, y = i / 8;
            px[i] = (x >= y && x >= (7 - y)) ? Color.gray : Color.clear;
        }
        tex.SetPixels(px);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(1f, 0.5f), 8f);
        obj.transform.localScale = Vector3.one * tipSize;
        return obj;
    }
}