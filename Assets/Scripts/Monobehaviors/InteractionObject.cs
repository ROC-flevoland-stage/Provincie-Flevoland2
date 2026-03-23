using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class InteractionObject : MonoBehaviour
{
    [SerializeField] private SphereCollider interactionCollider;
    [SerializeField] private GameObject canvas;
    [SerializeField] private UnityEvent interactActions;
    public bool interactable = false;

    private void Start()
    {
        interactable = false;

        // Try to find a collider if not assigned
        if (!interactionCollider) interactionCollider = GetComponent<SphereCollider>();
        if (!interactionCollider)
        {
            Debug.LogError($"No collider found for interactable object {name}. Disabling interaction.");
            return;
        }
        interactionCollider.isTrigger = true;

        // Try to find a canvas in children if not assigned
        if (!canvas) canvas = GetComponentInChildren<Canvas>(true).gameObject;

        // Hide the canvas at start
        if (!canvas) Debug.LogWarning($"No canvas found for interactable object {name}");
        else canvas.SetActive(false);
    }

    private void Update()
    {
        if (!interactable) return;

        // Check if "e" is pressed. If so, invoke interaction actions.
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // Nieuwe Input System: Keyboard
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.eKey.wasPressedThisFrame)
            {
                Interact();
            }
        }
#else
        // Oude Input Manager
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
#endif
    }

    /// <summary>
    /// Invoke all interaction actions assigned to this object.
    /// </summary>
    public void Interact()
    {
        interactActions?.Invoke();
    }

    /// <summary>
    /// Show or hide the interaction UI.
    /// </summary>
    /// <param name="show">Whether to show or hide the UI.</param>
    private void ShowUi(bool show)
    {
        canvas.SetActive(show);
    }

    /// <summary>
    /// Show the interaction UI when the player enters the trigger area and the object is interactable.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactable = true;
            ShowUi(true);
        }
    }

    /// <summary>
    /// Hide the interaction UI when the player exits the trigger area and the object is interactable.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactable = false;
            ShowUi(false);
        }

    }
}
