using UnityEngine;

public class GasBooster : MonoBehaviour
{
    [Header("Gas Tank")]
    public float maxGas = 100f;
    public float currentGas = 100f;

    [Header("Continuous Boost (Hold Space)")]
    public float gasConsumptionRate = 15f;
    public float hookedBoostForce = 35f;
    public float airBoostForce = 10f;
    public float groundBoostForce = 4f;

    [Header("Burst (Tap Space)")]
    public float burstImpulse = 10f;
    public float burstCost = 8f;
    public float burstCooldown = 0.3f;

    [Header("Limits")]
    public float maxSpeed = 35f;

    [Header("Visual")]
    public ParticleSystem gasParticlesLeft;
    public ParticleSystem gasParticlesRight;

    [Header("Audio")]
    public AudioClip gasLoopSound;
    public AudioClip gasBurstSound;

    private PlayerMovement2D player;
    private GrapplingHook hook;
    private Rigidbody2D rb;
    private AudioSource gasAudio;
    private float burstTimer;
    private bool wasUsingGas;

    private void Awake()
    {
        player = GetComponent<PlayerMovement2D>();
        hook = GetComponent<GrapplingHook>();
        rb = GetComponent<Rigidbody2D>();

        gasAudio = gameObject.AddComponent<AudioSource>();
        gasAudio.loop = true;
        gasAudio.playOnAwake = false;
    }

    private void Start()
    {
        currentGas = maxGas;

        if (gasParticlesLeft == null)
            gasParticlesLeft = CreateGasParticles("GasLeft", new Vector3(-0.3f, -0.2f, 0));
        if (gasParticlesRight == null)
            gasParticlesRight = CreateGasParticles("GasRight", new Vector3(0.3f, -0.2f, 0));
    }

    private void Update()
    {
        burstTimer -= Time.deltaTime;

        bool usingGas = false;

        // ═══ Burst (tap Space no ar) ═══
        if (Input.GetKeyDown(KeyCode.Space) && currentGas >= burstCost
            && burstTimer <= 0f && !player.isGrounded)
        {
            DoBurst();
        }

        // ═══ Continuous (hold Space) ═══
        if (Input.GetKey(KeyCode.Space) && currentGas > 0f)
        {
            usingGas = true;
        }

        UpdateParticles(usingGas);
        UpdateAudio(usingGas);
        wasUsingGas = usingGas;
    }

    private void FixedUpdate()
    {
        if (!Input.GetKey(KeyCode.Space) || currentGas <= 0f)
            return;

        // Consumir gás
        currentGas -= gasConsumptionRate * Time.fixedDeltaTime;
        currentGas = Mathf.Max(0f, currentGas);
        if (currentGas <= 0f) return;

        Vector2 boostDir;
        float force;

        if (player.isHooked && hook.currentState == GrapplingHook.HookState.Attached)
        {
            // ═══════════════════════════════════════
            // NO GANCHO: boost tangencial
            // A/D definem a direção do boost, NÃO movem diretamente
            // ═══════════════════════════════════════
            boostDir = GetTangentialDirection(player.intendedDirection);
            force = hookedBoostForce;
        }
        else if (player.isGrounded)
        {
            // ═══ NO CHÃO: fraco, horizontal ═══
            boostDir = GetHorizontalDirection();
            force = groundBoostForce;
        }
        else
        {
            // ═══ NO AR SEM GANCHO: médio, horizontal ═══
            boostDir = GetHorizontalDirection();
            force = airBoostForce;
        }

        rb.AddForce(boostDir * force, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    private void DoBurst()
    {
        currentGas -= burstCost;
        burstTimer = burstCooldown;

        Vector2 burstDir;

        if (player.isHooked)
            burstDir = GetTangentialDirection(player.intendedDirection);
        else
            burstDir = GetHorizontalDirection();

        rb.AddForce(burstDir * burstImpulse, ForceMode2D.Impulse);

        if (gasBurstSound != null)
            AudioSource.PlayClipAtPoint(gasBurstSound, transform.position);

        if (gasParticlesLeft != null) gasParticlesLeft.Emit(20);
        if (gasParticlesRight != null) gasParticlesRight.Emit(20);
    }

    // ═══════════════════════════════════════════
    // DIREÇÕES
    // ═══════════════════════════════════════════

    /// <summary>
    /// Calcula a direção tangencial ao pêndulo.
    /// 
    /// intendedDir: -1 (esquerda), 0 (neutro), 1 (direita)
    /// 
    /// A tangente é perpendicular à corda.
    /// Escolhemos a tangente que mais se alinha com a direção desejada.
    /// Se neutro, usamos a direção do movimento atual.
    /// </summary>
    private Vector2 GetTangentialDirection(int intendedDir)
    {
        // Vetor do JOGADOR para o PIVOT (aponta para o pivot)
        Vector2 toPivot = (hook.attachPoint - rb.position);
        float dist = toPivot.magnitude;

        if (dist < 0.01f)
            return player.facingRight ? Vector2.right : Vector2.left;

        Vector2 ropeDir = toPivot / dist; // normalizado, aponta para o pivot

        // Duas tangentes possíveis (perpendiculares à corda):
        // tangentRight = rotação 90° horária de ropeDir → tende para a direita
        // tangentLeft  = rotação 90° anti-horária       → tende para a esquerda
        Vector2 tangentRight = new Vector2(ropeDir.y, -ropeDir.x);
        Vector2 tangentLeft = new Vector2(-ropeDir.y, ropeDir.x);

        // Verificar qual tangente realmente aponta para a direita
        // (pode inverter dependendo de onde o pivot está)
        if (tangentRight.x < 0)
        {
            // Trocar se tangentRight na verdade aponta pra esquerda
            Vector2 temp = tangentRight;
            tangentRight = tangentLeft;
            tangentLeft = temp;
        }

        if (intendedDir > 0)
            return tangentRight;
        else if (intendedDir < 0)
            return tangentLeft;

        // Neutro: boost na direção do movimento atual
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            Vector2 vel = rb.linearVelocity.normalized;
            // Projetar velocidade na tangente mais próxima
            if (Vector2.Dot(vel, tangentRight) >= 0)
                return tangentRight;
            else
                return tangentLeft;
        }

        // Fallback: direção que o personagem está olhando
        return player.facingRight ? tangentRight : tangentLeft;
    }

    /// <summary>
    /// Direção horizontal simples baseada no input ou facing.
    /// </summary>
    private Vector2 GetHorizontalDirection()
    {
        int dir = player.intendedDirection;

        if (dir > 0)
            return Vector2.right;
        else if (dir < 0)
            return Vector2.left;

        return player.facingRight ? Vector2.right : Vector2.left;
    }

    // ═══════════════════════════════════════════
    // PUBLIC
    // ═══════════════════════════════════════════
    public void RefillGas(float amount)
    {
        currentGas = Mathf.Min(currentGas + amount, maxGas);
    }

    public void RefillGasFull()
    {
        currentGas = maxGas;
    }

    public float GetGasPercentage()
    {
        return currentGas / maxGas;
    }

    // ═══════════════════════════════════════════
    // VISUALS
    // ═══════════════════════════════════════════
    private void UpdateParticles(bool active)
    {
        if (gasParticlesLeft == null) return;

        if (active && currentGas > 0f)
        {
            if (!gasParticlesLeft.isPlaying) gasParticlesLeft.Play();
            if (!gasParticlesRight.isPlaying) gasParticlesRight.Play();
        }
        else
        {
            if (gasParticlesLeft.isPlaying) gasParticlesLeft.Stop();
            if (gasParticlesRight.isPlaying) gasParticlesRight.Stop();
        }
    }

    private void UpdateAudio(bool active)
    {
        if (gasLoopSound == null) return;

        if (active && currentGas > 0f && !wasUsingGas)
        {
            gasAudio.clip = gasLoopSound;
            gasAudio.Play();
        }
        else if ((!active || currentGas <= 0f) && wasUsingGas)
        {
            gasAudio.Stop();
        }
    }

    private ParticleSystem CreateGasParticles(string name, Vector3 localPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = localPos;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.25f;
        main.startSpeed = 4f;
        main.startSize = 0.15f;
        main.startColor = new Color(1f, 1f, 1f, 0.4f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 40;

        var emission = ps.emission;
        emission.rateOverTime = 25;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 12f;
        shape.radius = 0.03f;

        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f,
            new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0)));

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.gray, 1) },
            new[] { new GradientAlphaKey(0.5f, 0), new GradientAlphaKey(0f, 1) }
        );
        col.color = g;

        var rend = obj.GetComponent<ParticleSystemRenderer>();
        rend.material = new Material(Shader.Find("Sprites/Default"));
        rend.sortingOrder = 4;

        ps.Stop();
        return ps;
    }
}