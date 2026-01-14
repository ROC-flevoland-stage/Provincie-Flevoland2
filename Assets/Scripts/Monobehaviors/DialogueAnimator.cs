using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogueAnimator : MonoBehaviour
{
    private abstract class TextAnimation
    {
        public int startIndex;
        public int endIndex;
        public float startTime;
        public abstract void Apply(TMP_TextInfo textInfo, Vector3[][] original, Vector3[][] modified, float time);
    }

    private static DialogueAnimator _instance;

    public static DialogueAnimator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<DialogueAnimator>();
                if (_instance == null)
                {
                    GameObject animatorObject = new GameObject("DialogueAnimator");
                    _instance = animatorObject.AddComponent<DialogueAnimator>();
                }
            }
            return _instance;
        }
    }

    private Dictionary<TextMeshProUGUI, Vector3[][]> originalVertices = new();
    private Dictionary<TextMeshProUGUI, Vector3[][]> modifiedVertices = new();
    private Dictionary<TextMeshProUGUI, List<TextAnimation>> animations = new();
    private HashSet<TextMeshProUGUI> pendingInitialization = new();

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (animations.Count == 0) return;

        foreach (var kvp in animations)
        {
            // Check if initialization is pending for this textMesh
            if (pendingInitialization.Contains(kvp.Key)) continue;

            TextMeshProUGUI textMesh = kvp.Key;
            List<TextAnimation> activeAnimations = kvp.Value;

            Vector3[][] original = originalVertices[textMesh];
            Vector3[][] modified = modifiedVertices[textMesh];

            textMesh.ForceMeshUpdate();

            var textInfo = textMesh.textInfo;

            // Rebuild working vertices from originals
            for (int m = 0; m < original.Length; m++)
                Array.Copy(original[m], modified[m], original[m].Length);

            float time = Time.time;

            // Apply all animations into modifiedVertices
            foreach (var anim in activeAnimations)
                anim.Apply(textInfo, original, modified, time);

            // Push vertices to mesh
            for (int m = 0; m < textInfo.meshInfo.Length; m++)
            {
                var mesh = textInfo.meshInfo[m].mesh;

                mesh.vertices = modified[m];
                mesh.colors32 = textInfo.meshInfo[m].colors32;

                textMesh.UpdateGeometry(mesh, m);
            }

            // Put local copies back
            originalVertices[textMesh] = original;
            modifiedVertices[textMesh] = modified;
        }

        // Perform pending initializations after TMP rebuilds
        if (pendingInitialization.Count > 0)
        {
            foreach (var textMesh in pendingInitialization)
            {
                textMesh.ForceMeshUpdate();
                var textInfo = textMesh.textInfo;

                Vector3[][] original = new Vector3[textInfo.meshInfo.Length][];
                Vector3[][] modified = new Vector3[textInfo.meshInfo.Length][];

                for (int m = 0; m < textInfo.meshInfo.Length; m++)
                {
                    original[m] = textInfo.meshInfo[m].vertices.Clone() as Vector3[];
                    modified[m] = textInfo.meshInfo[m].vertices.Clone() as Vector3[];
                }

                originalVertices[textMesh] = original;
                modifiedVertices[textMesh] = modified;
            }

            pendingInitialization.Clear();
        }
    }

    public void StopAllAnimations(TextMeshProUGUI textMesh = null)
    {
        // If a textMesh is provided, stop all animations for that specific textMesh
        if (textMesh)
        {
            if (!animations.ContainsKey(textMesh)) return;
            // Restore original vertices
            RestoreOriginalVertices(textMesh);
            // Clear data for the textMesh
            originalVertices.Remove(textMesh);
            modifiedVertices.Remove(textMesh);
            animations.Remove(textMesh);
            return;
        }
        // Otherwise, stop all animations
        // Restore original vertices for all textMeshes
        foreach (var kvp in animations)
            RestoreOriginalVertices(kvp.Key);
        // Clear all data
        originalVertices.Clear();
        modifiedVertices.Clear();
        animations.Clear();
    }

    private void RestoreOriginalVertices(TextMeshProUGUI textMesh)
    {
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;
        for (int m = 0; m < textInfo.meshInfo.Length; m++)
        {
            textInfo.meshInfo[m].mesh.vertices = originalVertices[textMesh][m];
            textInfo.meshInfo[m].mesh.colors32 = textInfo.meshInfo[m].colors32;
            textMesh.UpdateGeometry(textInfo.meshInfo[m].mesh, m);
        }
        textMesh.ForceMeshUpdate();
    }

    private void InitializeTextMesh(TextMeshProUGUI textMesh)
    {
        pendingInitialization.Add(textMesh);
    }

    /// <summary>
    /// Starts the shake text animation.
    /// Takes these parameters:
    /// <list type="bullet">
    /// <item><description>magnitude: A float value representing the shake intensity</description></item>
    /// </list>
    /// </summary>
    /// <param name="textMesh">The TextMesh to animate.</param>
    /// <param name="startIndex">The start index of the text to animate.</param>
    /// <param name="endIndex">The end index of the text to animate.</param>
    /// <param name="parameters">A dictionary of parameters for the animation.</param>
    public static void StartTextShakeAnimation(TextMeshProUGUI textMesh, int startIndex, int endIndex, Dictionary<string, object> parameters)
    {
        // Get magnitude parameter with default
        float magnitude = parameters.ContainsKey("magnitude") ? (float)parameters["magnitude"] : 1f;

        // Start the animation
        if (!Instance.animations.ContainsKey(textMesh))
        {
            Instance.animations[textMesh] = new List<TextAnimation>();
            Instance.InitializeTextMesh(textMesh);
        }
        Instance.animations[textMesh].Add(new TextShakeAnimation(startIndex, endIndex, magnitude));
    }

    private class TextShakeAnimation : TextAnimation
    {
        public float magnitude;

        public TextShakeAnimation(int startIndex, int endIndex, float magnitude)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.magnitude = magnitude;
        }

        public override void Apply(TMP_TextInfo textInfo, Vector3[][] original, Vector3[][] modified, float time)
        {
            // Loop through each character from startIndex to endIndex
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Skip invisible characters
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the character and vertices used by this character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Random shake offset
                Vector3 offset = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), 0);

                // Apply to 4 vertices of the character, since each character is a quad
                for (int j = 0; j < 4; j++)
                    modified[materialIndex][vertexIndex + j] += offset;
            }
        }
    }

    /// <summary>
    /// Starts the wave text animation.
    /// <list type="bullet">
    /// <item><description>amplitude: A float value representing how far each character moves from it's original position</description></item>
    /// <item><description>angle: A float value representing the angle at which the characters move</description></item>
    /// <item><description>speed: A float value representing how quickly a character completes a full wave cycle</description></item>
    /// <item><description>delay: A float value representing the delay between each character's wave start time</description></item>
    /// </list>
    /// </summary>
    /// <param name="textMesh">The TextMesh to animate.</param>
    /// <param name="startIndex">The start index of the text to animate.</param>
    /// <param name="endIndex">The end index of the text to animate.</param>
    /// <param name="parameters">A dictionary of parameters for the animation.</param>
    public static void StartTextWaveAnimation(TextMeshProUGUI textMesh, int startIndex, int endIndex, Dictionary<string, object> parameters)
    {
        // Get all parameters with defaults
        float amplitude = parameters.ContainsKey("amplitude") ? (float)parameters["amplitude"] : 10f;
        float angle = parameters.ContainsKey("angle") ? (float)parameters["angle"] : 90f;
        float speed = parameters.ContainsKey("speed") ? (float)parameters["speed"] : 1f;
        float delay = parameters.ContainsKey("delay") ? (float)parameters["delay"] : 0.1f;

        // Start the animation
        if (!Instance.animations.ContainsKey(textMesh))
        {
            Instance.animations[textMesh] = new List<TextAnimation>();
            Instance.InitializeTextMesh(textMesh);
        }
        Instance.animations[textMesh].Add(new TextWaveAnimation(startIndex, endIndex, amplitude, angle, speed, delay));
    }

    private class TextWaveAnimation : TextAnimation
    {
        public float amplitude;
        public float angle;
        public float speed;
        public float delay;

        public TextWaveAnimation(int startIndex, int endIndex, float amplitude, float angle, float speed, float delay)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.amplitude = amplitude;
            this.angle = angle * Mathf.Deg2Rad;
            this.speed = speed;
            this.delay = delay;
            this.startTime = Time.time;
        }

        public override void Apply(TMP_TextInfo textInfo, Vector3[][] original, Vector3[][] modified, float time)
        {
            // Loop through each character from startIndex to endIndex
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Skip invisible characters
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the character and vertices used by this character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Calculate wave offset
                float elapsed = time - startTime - (i * delay);
                float wave = Mathf.Sin(elapsed * speed) * amplitude;
                Vector3 offset = new Vector3(wave * Mathf.Cos(angle), wave * Mathf.Sin(angle), 0);

                // Apply to 4 vertices of the character, since each character is a quad
                for (int j = 0; j < 4; j++)
                    modified[materialIndex][vertexIndex + j] += offset;
            }
        }
    }

    /// <summary>
    /// Starts the spin text animation.
    /// <list type="bullet">
    /// <item><description>speed: A float value representing how quickly a character spins clockswise around its center</description></item>
    /// </list>
    /// </summary>
    /// <param name="textMesh">The TextMesh to animate.</param>
    /// <param name="startIndex">The start index of the text to animate.</param>
    /// <param name="endIndex">The end index of the text to animate.</param>
    /// <param name="parameters">A dictionary of parameters for the animation.</param>
    public static void StartTextSpinAnimation(TextMeshProUGUI textMesh, int startIndex, int endIndex, Dictionary<string, object> parameters)
    {
        // Get speed parameter with default
        float speed = parameters.ContainsKey("speed") ? (float)parameters["speed"] : 1f;

        // Start the animation
        if (!Instance.animations.ContainsKey(textMesh))
        {
            Instance.animations[textMesh] = new List<TextAnimation>();
            Instance.InitializeTextMesh(textMesh);
        }
        Instance.animations[textMesh].Add(new TextSpinAnimation(startIndex, endIndex, speed));
    }

    private class TextSpinAnimation : TextAnimation
    {
        public float speed;

        public TextSpinAnimation(int startIndex, int endIndex, float speed)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.speed = speed;
            this.startTime = Time.time;
        }

        public override void Apply(TMP_TextInfo textInfo, Vector3[][] original, Vector3[][] modified, float time)
        {
            // Loop through each character from startIndex to endIndex  
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Skip invisible characters  
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the character and vertices used by this character.  
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Calculate spin angle  
                float angle = (time - startTime) * speed * 360f;
                float rad = angle * Mathf.Deg2Rad;

                // Calculate the center of the character  
                Vector3 center = (original[materialIndex][vertexIndex + 0] +
                                  original[materialIndex][vertexIndex + 1] +
                                  original[materialIndex][vertexIndex + 2] +
                                  original[materialIndex][vertexIndex + 3]) / 4f;

                // Create a rotation matrix for the spin  
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                // Rotate each vertex around the center  
                for (int j = 0; j < 4; j++)
                {
                    Vector3 vertex = original[materialIndex][vertexIndex + j];
                    modified[materialIndex][vertexIndex + j] = center + rotation * (vertex - center);
                }
            }
        }
    }

    /// <summary>
    /// Starts the color text animation.
    /// <list type="bullet">
    /// <item><description>color1: A string value representing the starting color in hex format</description></item>
    /// <item><description>color2: A string value representing the ending color in hex format</description></item>
    /// <item><description>speed: A float value representing how quickly the color transitions between color1 and color2</description></item>
    /// </list>
    /// </summary>
    /// <param name="textMesh">The TextMesh to animate.</param>
    /// <param name="startIndex">The start index of the text to animate.</param>
    /// <param name="endIndex">The end index of the text to animate.</param>
    /// <param name="parameters">A dictionary of parameters for the animation.</param>
    public static void StartTextColorAnimation(TextMeshProUGUI textMesh, int startIndex, int endIndex, Dictionary<string, object> parameters)
    {
        // Check if required parameters are present
        if (!parameters.TryGetValue("color1", out object c1))
            Debug.LogError("Color1 parameter missing for color text animation.");
        if (!parameters.TryGetValue("color2", out object c2))
            Debug.LogError("Color2 parameter missing for color text animation.");

        // Try to parse colors
        if (!ColorUtility.TryParseHtmlString(c1.ToString(), out Color color1))
            Debug.LogError($"Invalid color string for color1: {c1}");
        if (!ColorUtility.TryParseHtmlString(c2.ToString(), out Color color2))
            Debug.LogError($"Invalid color string for color2: {c2}");

        // Get speed parameter with default
        float speed = parameters.ContainsKey("speed") ? (float)parameters["speed"] : 1f;

        // Start the animation
        if (!Instance.animations.ContainsKey(textMesh))
        {
            Instance.animations[textMesh] = new List<TextAnimation>();
            Instance.InitializeTextMesh(textMesh);
        }
        Instance.animations[textMesh].Add(new TextColorAnimation(startIndex, endIndex, color1, color2, speed));
    }

    private class TextColorAnimation : TextAnimation
    {
        public Color color1;
        public Color color2;
        public float speed;

        public TextColorAnimation(int startIndex, int endIndex, Color color1, Color color2, float speed)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.color1 = color1;
            this.color2 = color2;
            this.speed = speed;
            this.startTime = Time.time;
        }

        public override void Apply(TMP_TextInfo textInfo, Vector3[][] original, Vector3[][] modified, float time)
        {
            // Loop through each character from startIndex to endIndex
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Skip invisible characters
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the character and vertices used by this character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Calculate color based on time  
                float t = (Mathf.Sin((time - startTime) * speed) + 1f) / 2f;
                Color currentColor = Color.Lerp(color1, color2, t);
                Color32 currentColor32 = currentColor;

                // Apply color to vertex colors  
                for (int j = 0; j < 4; j++)
                    textInfo.meshInfo[materialIndex].colors32[vertexIndex + j] = currentColor32;
            }
        }
    }

    /// <summary>
    /// Starts the pulse text animation.
    /// <list type="bullet">
    /// <item><description>size: A float value representing a multiplier for the text's original size</description></item>
    /// <item><description>speed: A float value representing how quickly the text scales up and down</description></item>
    /// <item><description>delay: A float value representing the delay between each character's pulse start time</description></item>
    /// </list>
    /// </summary>
    /// <param name="textMesh">The TextMesh to animate.</param>
    /// <param name="startIndex">The start index of the text to animate.</param>
    /// <param name="endIndex">The end index of the text to animate.</param>
    /// <param name="parameters">A dictionary of parameters for the animation.</param>
    public static void StartTextPulseAnimation(TextMeshProUGUI textMesh, int startIndex, int endIndex, Dictionary<string, object> parameters)
    {
        // Get all parameters with defaults
        float size = parameters.ContainsKey("size") ? (float)parameters["size"] : 1.5f;
        float speed = parameters.ContainsKey("speed") ? (float)parameters["speed"] : 1.5f;
        float delay = parameters.ContainsKey("delay") ? (float)parameters["delay"] : 0.2f;

        // Start the animation
        if (!Instance.animations.ContainsKey(textMesh))
        {
            Instance.animations[textMesh] = new List<TextAnimation>();
            Instance.InitializeTextMesh(textMesh);
        }
        Instance.animations[textMesh].Add(new TextPulseAnimation(startIndex, endIndex, size, speed, delay));
    }

    private class TextPulseAnimation : TextAnimation
    {
        public float maxSize;
        public float minSize;
        public float speed;
        public float delay;

        public TextPulseAnimation(int startIndex, int endIndex, float maxSize, float speed, float delay)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.maxSize = maxSize;
            this.minSize = 1f / maxSize;
            this.speed = speed;
            this.delay = delay;
        }

        public override void Apply(TMP_TextInfo textInfo, Vector3[][] original, Vector3[][] modified, float time)
        {
            // Loop through each character from startIndex to endIndex
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Skip invisible characters
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the character and vertices used by this character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Calculate pulse scale
                Vector3 center = (original[materialIndex][vertexIndex + 0] +
                                  original[materialIndex][vertexIndex + 1] +
                                  original[materialIndex][vertexIndex + 2] +
                                  original[materialIndex][vertexIndex + 3]) / 4f;
                float t = (Mathf.Sin((time - startTime - (delay * i)) * speed) + 1f) * 0.5f;
                float size = Mathf.Lerp(minSize, maxSize, t);

                // Apply to 4 vertices of the character, since each character is a quad
                for (int j = 0; j < 4; j++)
                {
                    Vector3 basePos = original[materialIndex][vertexIndex + j];
                    modified[materialIndex][vertexIndex + j] = center + (basePos - center) * size;
                }
            }
        }
    }
}
