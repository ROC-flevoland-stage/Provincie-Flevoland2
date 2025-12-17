using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<DialogueManager>();
                if (_instance == null)
                    Debug.LogError("No dialogueManager instance found in the scene.");
                else
                    DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    [Header("State Variables")]
    [SerializeField] private bool isDialogueActive = false;         // Is a dialogue currently active
    [SerializeField] private bool isDialogueReady = false;          // Is the textbox ready for input
    [SerializeField] private bool isChoiceActive = false;           // Is a choicebox currently active

    [Header("Prefabs")]
    [SerializeField] private GameObject textBoxPrefab;              // Prefab for the textbox UI
    [SerializeField] private GameObject choiceBoxPrefab;            // Prefab for the choicebox UI
    [SerializeField] private GameObject choiceButtonPrefab;         // Prefab for the choice button UI

    [Header("Text/Choicebox Variables")]
    [SerializeField] private DialogueTree currentdialogue;          // Current dialogue tree
    [SerializeField] private Transform textBox;                     // Transform of the textbox UI
    [SerializeField] private Transform choiceBox;                   // Transform of the choicebox UI
    [SerializeField] private Transform canvas;                      // Transform of the main canvas
    [SerializeField] private TextMeshProUGUI textMesh;              // TextMeshPro component for displaying dialogue text

    // Public properties
    public bool IsDialogueActive => isDialogueActive;
    public bool IsDialogueReady => isDialogueReady;
    public bool IsChoiceActive => isChoiceActive;
    public TextMeshProUGUI TextMesh => textMesh;

    public event Action<string> OnDialogueLineChanged;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Sets the text of the TextMeshPro component to the current line in the dialogue and invokes the OnDialogueLineChanged event.
    /// </summary>
    public void SetTextMeshText()
    {
        textMesh.text = currentdialogue.CurrentLine;
        OnDialogueLineChanged?.Invoke(currentdialogue.CurrentLine);
    }

    /// <summary>
    /// Shows the next line in the current dialogue.
    /// </summary>
    public void ShowNextLine()
    {
        if (!isDialogueReady || isChoiceActive) return;
        DialogueAnimator.Instance.StopAllAnimations(textMesh);
        currentdialogue.Next();
        if (currentdialogue) SetTextMeshText();
    }

    /// <summary>
    /// Shows the previous line in the current dialogue.
    /// </summary>
    public void ShowPreviousLine()
    {
        if (!isDialogueReady || isChoiceActive) return;
        DialogueAnimator.Instance.StopAllAnimations(textMesh);
        currentdialogue.Previous();
        SetTextMeshText();
    }

    /// <summary>
    /// Reloads the current line in the dialogue.
    /// </summary>
    public void ReloadLine()
    {
        if (!isDialogueReady) return;
        SetTextMeshText();
    }

    /// <summary>
    /// Starts a dialogue using the provided dialogue tree.
    /// </summary>
    /// <param name="dialogue"></param>
    public void Startdialogue(DialogueTree dialogue)
    {
        isDialogueActive = true;
        isDialogueReady = false;
        isChoiceActive = false;

        // Set the current dialogue tree
        currentdialogue = dialogue;

        // Get the canvas if it doesn't exist
        if (canvas == null)
            canvas = FindAnyObjectByType<Canvas>().transform;
        // if canvas is still null, create one
        if (canvas == null)
        {
            canvas = new GameObject("Canvas").transform;
            canvas.gameObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        // Instantiate the textbox if it doesn't exist
        if (textBox == null)
        {
            textBox = Instantiate(textBoxPrefab, canvas).transform;
            textBox.localScale = Vector3.zero; // Start closed
        }

        if (textMesh == null)
            textMesh = textBox.Find("TextArea").GetComponent<TMPro.TextMeshProUGUI>();

        // Instantiate the choicebox if it doesn't exist
        if (choiceBox == null)
        {
            choiceBox = Instantiate(choiceBoxPrefab, textBox).transform;
            choiceBox.localScale = Vector3.zero; // Start closed
        }

        // Open the textbox UI
        StartCoroutine(OpenTextbox());
    }

    /// <summary>
    /// Stops the current dialogue.
    /// </summary>
    public void Stopdialogue()
    {
        isDialogueActive = false;
        isDialogueReady = false;
        isChoiceActive = false;

        currentdialogue = null;
        StartCoroutine(OpenTextbox(true));
    }

    /// <summary>
    /// Shows the choices for the current dialogue.
    /// </summary>
    public void ShowChoices()
    {
        StartCoroutine(OpenChoiceBox());
    }

    /// <summary>
    /// Hides the choices for the current dialogue.
    /// </summary>
    public void HideChoices()
    {
        StartCoroutine(OpenChoiceBox(true));
    }

    /// <summary>
    /// Opens the textbox UI over a small period of time.
    /// </summary>
    /// <param name="close">Closes the textbox instead of opening it.</param>
    private IEnumerator OpenTextbox(bool close=false)
    {
        textMesh.text = "";

        // Set the initial scale
        float duration = 0.25f; // Duration of the animation
        float elapsed = 0f;
        Vector3 startScale = close ? Vector3.one : Vector3.zero;
        Vector3 endScale = close ? Vector3.zero : Vector3.one;
        textBox.transform.localScale = startScale;

        // Animate the scale over time
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            textBox.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        textBox.transform.localScale = endScale;

        if (close) yield break;

        // Set the text
        textMesh.text = currentdialogue.CurrentLine;
        isDialogueReady = true;
    }

    /// <summary>
    /// Opens the choicebox UI over a small period of time.
    /// </summary>
    /// <param name="close">Closes the choicebox instead of opening it.</param>
    private IEnumerator OpenChoiceBox(bool close = false)
    {
        isChoiceActive = true;

        // Set the initial scale
        float duration = 0.25f; // Duration of the animation
        float elapsed = 0f;
        Vector3 startScale = close ? Vector3.one : Vector3.zero;
        Vector3 endScale = close ? Vector3.zero : Vector3.one;
        choiceBox.transform.localScale = startScale;
        // Animate the scale over time
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            choiceBox.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        choiceBox.transform.localScale = endScale;

        if (close)
        {
            isChoiceActive = false;
            yield break;
        }

        // Set the choices
        Transform choiceArea = choiceBox.Find("Choices");
        foreach (Transform child in choiceArea)
            Destroy(child.gameObject);

        int buttonSpacing = 60;
        for (int i = 0; i < currentdialogue.Choices.Count; i++)
        {
            KeyValuePair<int, string> kvp = currentdialogue.Choices.ToArray()[i];
            int choiceIndex = kvp.Key;
            string choiceText = kvp.Value;

            // Instantiate a button for each choice
            Transform buttonTransform = Instantiate(choiceButtonPrefab, choiceArea).transform;
            TMPro.TextMeshProUGUI buttonText = buttonTransform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            buttonText.text = choiceText;

            // Get the button component and add a listener for selecting the choice and hiding the ChoiceBox
            Button buttonComponent = buttonTransform.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => currentdialogue.ChangeChoiceIndex(choiceIndex));
            buttonComponent.onClick.AddListener(() => HideChoices());

            // Position the button
            RectTransform buttonRect = buttonTransform.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(0, -buttonSpacing * i);
        }
    }
}