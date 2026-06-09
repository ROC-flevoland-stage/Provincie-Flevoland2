using UnityEngine;

public class SkinMaterialCycle : MonoBehaviour
{
    [Header("Objects to reskin")]
    public Renderer[] targetRenderers;  // Objects hier

    [Header("Available Skin Materials")]
    public Material[] skinMaterials;    // materials hier

    private int currentIndex = 0;

    void Start()
    {
        ApplyMaterial();
    }
    // Cycles door de materialen in de array en precenteert deze op de speler, dit kan worden aangeroepen vanuit de UI knoppen
    public void NextMaterial()
    {
        currentIndex++;
        if (currentIndex >= skinMaterials.Length)
            currentIndex = 0;

        ApplyMaterial();
    }

    public void PreviousMaterial()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = skinMaterials.Length - 1;

        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        foreach (Renderer rend in targetRenderers)
        {
            rend.material = skinMaterials[currentIndex];
        }
    }
}
