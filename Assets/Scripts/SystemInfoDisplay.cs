using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SystemInfoDisplay : MonoBehaviour
{
    // Singleton instance
    private static SystemInfoDisplay instance;

    [Header("Display Settings")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private bool showDeltaTime = true;
    [SerializeField] private bool showFrameRateSettings = true;
    [SerializeField] private bool showMemoryUsage = true;
    [SerializeField] private bool showSystemInfo = true;
    [SerializeField] private bool showRng = true;
    [SerializeField] private bool showControls = true;

    [Header("Activation Settings")]
    [SerializeField] private KeyCode[] activationKeyCombination = new KeyCode[] { KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.I };
    [SerializeField] private float maxTimeBetweenKeyPresses = 1.0f;
    [SerializeField] private bool debugKeyPresses = false;

    [Header("Toggle Key Bindings")]
    [SerializeField] private KeyCode toggleFPSKey = KeyCode.F;
    [SerializeField] private KeyCode toggleDeltaTimeKey = KeyCode.D;
    [SerializeField] private KeyCode toggleFrameRateSettingsKey = KeyCode.R;
    [SerializeField] private KeyCode toggleMemoryKey = KeyCode.M;
    [SerializeField] private KeyCode toggleSystemInfoKey = KeyCode.S;
    [SerializeField] private KeyCode toggleAllKey = KeyCode.A;
    [SerializeField] private KeyCode toggleControlsKey = KeyCode.C;
    [SerializeField] private KeyCode toggleRngKey = KeyCode.G;
    [SerializeField] private KeyCode increaseVSyncKey = KeyCode.PageUp;
    [SerializeField] private KeyCode decreaseVSyncKey = KeyCode.PageDown;

    [Header("UI Settings")]
    [SerializeField] private int fontSize = 20;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector2 offset = new Vector2(10, 10);

    [Header("Performance")]
    [SerializeField] private float updateInterval = 0.5f;

    private float fps;
    private float accum;
    private int frames;
    private float timeLeft;
    private StringBuilder statsText;
    private GUIStyle style;
    private Rect rect;

    // Key sequence tracking
    private List<KeyCode> pressedKeys = new List<KeyCode>();
    private float lastKeyPressTime = 0f;
    private bool isDisplayActive = false;

    private void Awake()
    {
        // Check if an instance already exists
        if (instance != null && instance != this)
        {
            // Another instance exists, destroy this one
            Debug.LogWarning("Multiple instances of SystemInfoDisplay detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Set this as the singleton instance
        instance = this;

        // Optional: Make the object persistent between scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;

        timeLeft = updateInterval;
        statsText = new StringBuilder(200);

        // Configure UI style
        style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        style.fontStyle = FontStyle.Bold;

        StartCoroutine(UpdateStats());
    }

    private void Update()
    {
        // Check for activation key combination
        CheckForKeySequence();

        // Only check for toggle keys if display is active
        if (isDisplayActive)
        {
            CheckToggleKeys();
        }

        if (!isDisplayActive)
            return;

        // Accumulate FPS calculations
        timeLeft -= Time.unscaledDeltaTime;
        accum += 1.0f / Time.unscaledDeltaTime;
        frames++;

        // Update FPS count every updateInterval seconds
        if (timeLeft <= 0.0f)
        {
            fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    private void CheckToggleKeys()
    {
        // Toggle specific display categories with key presses
        if (!IsHeld(KeyCode.LeftControl)) return;

        if (WasPressed(toggleFPSKey))
        {
            showFPS = !showFPS;
            ShowToggleMessage("FPS display", showFPS);
        }

        if (WasPressed(toggleDeltaTimeKey))
        {
            showDeltaTime = !showDeltaTime;
            ShowToggleMessage("Delta Time display", showDeltaTime);
        }

        if (WasPressed(toggleFrameRateSettingsKey))
        {
            showFrameRateSettings = !showFrameRateSettings;
            ShowToggleMessage("Frame Rate Settings display", showFrameRateSettings);
        }

        if (WasPressed(toggleMemoryKey))
        {
            showMemoryUsage = !showMemoryUsage;
            ShowToggleMessage("Memory Usage display", showMemoryUsage);
        }

        if (WasPressed(toggleSystemInfoKey))
        {
            showSystemInfo = !showSystemInfo;
            ShowToggleMessage("System Info display", showSystemInfo);
        }

        if (WasPressed(toggleRngKey))
        {
            showRng = !showRng;
            ShowToggleMessage("RNG seed", showRng);
        }

        // Toggle controls display
        if (WasPressed(toggleControlsKey))
        {
            showControls = !showControls;
            ShowToggleMessage("Controls display", showControls);
        }

        // Toggle all displays at once (except controls)
        if (WasPressed(toggleAllKey))
        {
            bool allOn = showFPS && showDeltaTime && showFrameRateSettings && showMemoryUsage && showSystemInfo && showRng;

            showFPS = showDeltaTime = showFrameRateSettings = showMemoryUsage = showSystemInfo = !allOn;

            ShowToggleMessage("All displays", !allOn);
        }

        if (WasPressed(increaseVSyncKey))
        {
            ChangeVSyncMode(1);
        }
        else if (WasPressed(decreaseVSyncKey))
        {
            ChangeVSyncMode(-1);
        }

        if (WasPressed(KeyCode.UpArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + 0.1f, 0.1f, 5f); // Increase time scale
        }
        else if (WasPressed(KeyCode.DownArrow))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - 0.1f, 0.1f, 5f); // Decrease time scale
        }
    }

    private void ShowToggleMessage(string displayName, bool isEnabled)
    {
        if (debugKeyPresses)
            Debug.Log($"{displayName} {(isEnabled ? "enabled" : "disabled")}");
    }

    private void CheckForKeySequence()
    {
        // Reset sequence if too much time has passed since last key press
        if (pressedKeys.Count > 0 && Time.realtimeSinceStartup - lastKeyPressTime > maxTimeBetweenKeyPresses)
        {
            if (debugKeyPresses)
                Debug.Log("Key sequence timed out, resetting");
            pressedKeys.Clear();
        }

        // Check for key presses in the combination
        for (int i = 0; i < activationKeyCombination.Length; i++)
        {
            KeyCode keyToCheck = activationKeyCombination[i];

            // If key was just pressed and it's the next key in the sequence
            if (WasPressed(keyToCheck))
            {
                if (pressedKeys.Count == i)
                {
                    pressedKeys.Add(keyToCheck);
                    lastKeyPressTime = Time.realtimeSinceStartup;

                    if (debugKeyPresses)
                        Debug.Log($"Key {keyToCheck} pressed. Sequence: {pressedKeys.Count}/{activationKeyCombination.Length}");

                    // If all keys in the sequence have been pressed
                    if (pressedKeys.Count == activationKeyCombination.Length)
                    {
                        ToggleDisplay();
                        pressedKeys.Clear();
                    }
                }
                else
                {
                    // Wrong key in sequence, reset
                    if (debugKeyPresses)
                        Debug.Log($"Incorrect key in sequence. Expected: {activationKeyCombination[pressedKeys.Count]}, Got: {keyToCheck}");
                    pressedKeys.Clear();
                }
            }
        }
    }

    private void ChangeVSyncMode(int change)
    {
        int newVSync = Mathf.Clamp(QualitySettings.vSyncCount + change, 0, 2);
        QualitySettings.vSyncCount = newVSync;

        string vsyncStatus = newVSync switch
        {
            0 => "Disabled",
            1 => "Every V Blank",
            2 => "Every Second V Blank",
            _ => "Unknown"
        };

        Debug.Log($"VSync Mode Changed: {vsyncStatus}");
    }

    private void ToggleDisplay()
    {
        isDisplayActive = !isDisplayActive;
        if (debugKeyPresses)
            Debug.Log($"System info display {(isDisplayActive ? "enabled" : "disabled")}");
    }

    private IEnumerator UpdateStats()
    {
        WaitForSeconds wait = new WaitForSeconds(updateInterval);

        while (true)
        {
            yield return wait;

            if (!isDisplayActive)
                continue;

            statsText.Clear();

            // Add controls help text
            if (showControls)
            {
                statsText.AppendLine("--- Controls ---");
                statsText.AppendLine($"{toggleFPSKey}: Toggle FPS | {toggleDeltaTimeKey}: Toggle Delta Time");
                statsText.AppendLine($"{toggleFrameRateSettingsKey}: Toggle Frame Rate | {toggleMemoryKey}: Toggle Memory");
                statsText.AppendLine($"{toggleSystemInfoKey}: Toggle System Info | {toggleAllKey}: Toggle All");
                statsText.AppendLine($"{toggleControlsKey}: Toggle Controls");
                statsText.AppendLine("---------------");
            }
            else
            {
                statsText.AppendLine($"Press {toggleControlsKey} for controls");
                statsText.AppendLine("---------------");
            }

            if (showFPS)
            {
                statsText.AppendLine($"FPS: {fps:F1}");
            }

            if (showDeltaTime)
            {
                statsText.AppendLine($"Delta Time: {Time.deltaTime * 1000:F1} ms");
                statsText.AppendLine($"Unscaled Delta Time: {Time.unscaledDeltaTime * 1000:F1} ms");
                statsText.AppendLine($"Time Scale: {Time.timeScale:F1}x");
            }

            if (showFrameRateSettings)
            {
                string vsyncStatus = QualitySettings.vSyncCount switch
                {
                    0 => "Disabled",
                    1 => "Every V Blank",
                    2 => "Every Second V Blank",
                    _ => $"Custom ({QualitySettings.vSyncCount})"
                };

                string targetFR = Application.targetFrameRate == -1 ?
                    "Unlimited" : Application.targetFrameRate.ToString();

                statsText.AppendLine($"Target Frame Rate: {targetFR}");
                statsText.AppendLine($"VSync: {vsyncStatus}");
                statsText.AppendLine($"TimeScale: {Time.timeScale:F1}x");
            }

            if (showRng)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                statsText.AppendLine($"RNG Seed: {UnityEngine.Random.seed}");
#pragma warning restore CS0618 // Type or member is obsolete
            }

            if (showMemoryUsage)
            {
                statsText.AppendLine($"Total Allocated Memory: {SystemInfo.systemMemorySize} MB");
                statsText.AppendLine($"Total Reserved Memory: {GC.GetTotalMemory(false) / 1048576f:F1} MB");
            }

            if (showSystemInfo)
            {
                statsText.AppendLine($"Device: {SystemInfo.deviceModel}");
                statsText.AppendLine($"OS: {SystemInfo.operatingSystem}");
                statsText.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
                statsText.AppendLine($"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
                statsText.AppendLine($"Graphics API: {SystemInfo.graphicsDeviceType}");
                statsText.AppendLine($"Graphics Memory: {SystemInfo.graphicsMemorySize} MB");
            }
        }
    }

    private void OnGUI()
    {
        if (!isDisplayActive)
            return;

        // Calculate rect based on text content and offset
        rect = new Rect(offset.x, offset.y, Screen.width / 3, Screen.height);

        // Draw background panel for better readability
        GUI.color = new Color(0, 0, 0, 0.5f);
        GUI.Box(new Rect(rect.x - 5, rect.y - 5,
                         rect.width - offset.x + 10,
                         (style.lineHeight) * (statsText.ToString().Split('\n').Length)), "");

        // Reset color and draw text
        GUI.color = Color.white;
        GUI.Label(rect, statsText.ToString(), style);
    }

    // Static method to check if instance exists (can be used by other scripts)
    public static bool HasInstance()
    {
        return instance != null;
    }

    private bool WasPressed(KeyCode key)
    {
        var kb = Keyboard.current;
        if (kb == null) return false;

        return key switch
        {
            KeyCode.LeftControl => kb.leftCtrlKey.wasPressedThisFrame,
            KeyCode.RightControl => kb.rightCtrlKey.wasPressedThisFrame,
            KeyCode.LeftShift => kb.leftShiftKey.wasPressedThisFrame,
            KeyCode.RightShift => kb.rightShiftKey.wasPressedThisFrame,
            KeyCode.F => kb.fKey.wasPressedThisFrame,
            KeyCode.D => kb.dKey.wasPressedThisFrame,
            KeyCode.R => kb.rKey.wasPressedThisFrame,
            KeyCode.M => kb.mKey.wasPressedThisFrame,
            KeyCode.S => kb.sKey.wasPressedThisFrame,
            KeyCode.A => kb.aKey.wasPressedThisFrame,
            KeyCode.C => kb.cKey.wasPressedThisFrame,
            KeyCode.G => kb.gKey.wasPressedThisFrame,
            KeyCode.I => kb.iKey.wasPressedThisFrame,
            KeyCode.PageUp => kb.pageUpKey.wasPressedThisFrame,
            KeyCode.PageDown => kb.pageDownKey.wasPressedThisFrame,
            KeyCode.UpArrow => kb.upArrowKey.wasPressedThisFrame,
            KeyCode.DownArrow => kb.downArrowKey.wasPressedThisFrame,
            _ => false
        };
    }

    private bool IsHeld(KeyCode key)
    {
        var kb = Keyboard.current;
        if (kb == null) return false;

        return key switch
        {
            KeyCode.LeftControl => kb.leftCtrlKey.isPressed,
            KeyCode.RightControl => kb.rightCtrlKey.isPressed,
            KeyCode.LeftShift => kb.leftShiftKey.isPressed,
            KeyCode.RightShift => kb.rightShiftKey.isPressed,
            _ => false
        };
    }
}
