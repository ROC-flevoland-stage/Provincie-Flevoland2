using UnityEngine;
using TMPro;

public class CubeText : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 1.2f, 0f);
    public float fontSize = 2f;
    public Color color = Color.white;

    TMP_Text tmp;
    Camera cam;
    string lastText;

    void Awake()
    {
        cam = Camera.main;

        tmp = GetComponentInChildren<TMP_Text>();
        if (tmp == null)
        {
            var go = new GameObject("CubeTextLabel");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = offset;
            go.transform.localRotation = Quaternion.identity;

            tmp = go.AddComponent<TextMeshPro>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
        }

        tmp.fontSize = fontSize;
        tmp.color = color;
        UpdateTextFromComponent();
    }

    void LateUpdate()
    {
        if (tmp != null)
            tmp.transform.localPosition = offset;

        if (cam != null && tmp != null)
        {
            var dir = tmp.transform.position - cam.transform.position;
            if (dir.sqrMagnitude > 0.0001f)
                tmp.transform.rotation = Quaternion.LookRotation(dir);
        }

        UpdateTextFromComponent();
    }

    void UpdateTextFromComponent()
    {
        var sf = GetComponent<SpawnedFile>();
        var text = (sf != null && !string.IsNullOrEmpty(sf.label)) ? sf.label : gameObject.name;
        if (text != lastText && tmp != null)
        {
            tmp.text = text;
            lastText = text;
        }
    }
}
