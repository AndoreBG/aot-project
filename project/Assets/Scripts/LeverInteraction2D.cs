using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class LeverInteraction2D : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    public bool interactOnlyOnce = true;
    public bool requirePlayerMovementComponent = true;
    public string playerTag = "Player";

    [Header("Prompt")]
    public GameObject interactionSymbol;
    public Vector3 autoSymbolOffset = new Vector3(0f, 1f, 0f);
    public bool createSymbolIfMissing = true;

    [Header("Events")]
    public UnityEvent onInteract;

    private bool playerInRange;
    private bool hasInteracted;
    private GameObject autoSymbol;

    public bool CanInteract => playerInRange && (!interactOnlyOnce || !hasInteracted);

    private void Awake()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;

        if (interactionSymbol == null && createSymbolIfMissing)
            interactionSymbol = CreateDefaultSymbol();

        SetSymbolVisible(false);
    }

    private void Update()
    {
        if (!CanInteract)
            return;

        if (Input.GetKeyDown(interactionKey))
            Interact();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInRange = true;
        SetSymbolVisible(CanInteract);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInRange = false;
        SetSymbolVisible(false);
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        hasInteracted = true;
        onInteract.Invoke();

        if (interactOnlyOnce)
            SetSymbolVisible(false);
    }

    public void ResetInteraction()
    {
        hasInteracted = false;
        SetSymbolVisible(CanInteract);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (requirePlayerMovementComponent)
            return other.GetComponentInParent<PlayerMovement2D>() != null;

        return other.GetComponentInParent<PlayerMovement2D>() != null || other.gameObject.tag == playerTag;
    }

    private void SetSymbolVisible(bool visible)
    {
        if (interactionSymbol != null)
            interactionSymbol.SetActive(visible);
    }

    private GameObject CreateDefaultSymbol()
    {
        GameObject symbol = new GameObject("Interaction Symbol");
        symbol.transform.SetParent(transform, false);
        symbol.transform.localPosition = autoSymbolOffset;

        TextMesh text = symbol.AddComponent<TextMesh>();
        text.text = interactionKey.ToString();
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.characterSize = 0.35f;
        text.fontSize = 48;
        text.color = Color.white;

        MeshRenderer renderer = symbol.GetComponent<MeshRenderer>();
        renderer.sortingOrder = 100;

        autoSymbol = symbol;
        return autoSymbol;
    }
}
