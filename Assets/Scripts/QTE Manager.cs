using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private Image timerLine;

    [Header("Settings")]
    [SerializeField] private int minInputs = 1;
    [SerializeField] private int maxInputs = 3;
    [SerializeField] private float timePerInput = 0.45f;

    private readonly KeyCode[] possibleKeys =
    {
        KeyCode.E,
        KeyCode.Space,
        KeyCode.A,
        KeyCode.D,
        KeyCode.W,
        KeyCode.S,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.J,
        KeyCode.K,
        KeyCode.Y,
        KeyCode.T,
        KeyCode.U,
        KeyCode.I,
    };

    private List<KeyCode> currentSequence = new List<KeyCode>();
    private int currentIndex;
    private float timer;
    private float totalTime;
    private bool active;

    private Action onSuccess;
    private Action onFail;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        qtePanel.SetActive(false);
    }
    private void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;
        timerLine.fillAmount = Mathf.Clamp01(timer / totalTime);

        if (timer <= 0f)
        {
            FailQTE();
            return;
        }

        KeyCode pressed = GetPressedKey();
        if (pressed == KeyCode.None) return;

        if (pressed == currentSequence[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= currentSequence.Count)
            {
                SuccessQTE();
            }
            else
            {
                UpdatePrompt();
            }
        }
        else
        {
            FailQTE();
        }
    }
    public void StartQTE(Action success, Action fail)
    {
        onSuccess = success;
        onFail = fail;

        currentSequence.Clear();
        currentIndex = 0;

        int inputCount = UnityEngine.Random.Range(minInputs, maxInputs + 1);
        for (int i = 0; i < inputCount; i++)
        {
            currentSequence.Add(possibleKeys[UnityEngine.Random.Range(0, possibleKeys.Length)]);
        }

        totalTime = inputCount * timePerInput;
        timer = totalTime;

        qtePanel.SetActive(true);
        active = true;

        UpdatePrompt();
        timerLine.fillAmount = 1f;
    }

    private void UpdatePrompt()
    {
        List<string> display = new List<string>();
        for (int i = 0; i < currentSequence.Count; i++)
        {
            display.Add(KeyToString(currentSequence[i]));
        }

        promptText.text = string.Join("  →  ", display);
    }

    private void SuccessQTE()
    {
        active = false;
        qtePanel.SetActive(false);
        onSuccess?.Invoke();
    }

    private void FailQTE()
    {
        active = false;
        qtePanel.SetActive(false);
        onFail?.Invoke();
    }

    private KeyCode GetPressedKey()
    {
        foreach (KeyCode key in possibleKeys)
        {
            if (Input.GetKeyDown(key))
                return key;
        }

        return KeyCode.None;
    }

    private string KeyToString(KeyCode key)
    {
        return key switch
        {
            KeyCode.Space => "SPACE",
            KeyCode.LeftArrow => "←",
            KeyCode.RightArrow => "→",
            KeyCode.UpArrow => "↑",
            KeyCode.DownArrow => "↓",
            _ => key.ToString().ToUpper()
        };
    }
}

