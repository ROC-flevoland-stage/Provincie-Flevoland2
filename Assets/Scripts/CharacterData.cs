using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [Header("Player Info")]
    public int age;
    public string gender;
    public string municipality;

    [Header("Appearance Materials")]
    public Material skinMaterial;
    public Material eyeMaterial;

    [Header("References to in-game objects")]
    public Renderer skinRenderer;
    public Renderer eyeRenderer;

    // Methode om materiale van de renderers te updaten
    public void UpdateMaterialsFromObjects()
    {
        if (skinRenderer != null)
            skinMaterial = skinRenderer.material;

        if (eyeRenderer != null)
            eyeMaterial = eyeRenderer.material;
    }

    // Verzamelening voor data van de speler die aangeroepen kan worden vanuit de letter P
    public void PrintToConsole()
    {

        UpdateMaterialsFromObjects();

        Debug.Log("=== Character Data ===");
        Debug.Log("Leeftijd: " + age);
        Debug.Log("Geslacht: " + gender);
        Debug.Log("Gemeente: " + municipality);
        Debug.Log("Skin Material: " + MaterialName(skinMaterial));
        Debug.Log("Eye Material: " + MaterialName(eyeMaterial));
    }

    private string MaterialName(Material mat)
    {
        return mat != null ? mat.name : "None";
    }
void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintToConsole();
        }
    }
}