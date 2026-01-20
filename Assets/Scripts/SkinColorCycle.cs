using UnityEngine;

public class SkinMaterialCycle : MonoBehaviour
{
    [Header("Objects to reskin")]
    public Renderer[] targetRenderers;  // Objects here

    [Header("Available Skin Materials")]
    public Material[] skinMaterials;    // materials here

    private int currentIndex = 0;

    void Start()
    {
        ApplyMaterial();
    }

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
