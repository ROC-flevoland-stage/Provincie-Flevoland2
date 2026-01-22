using TMPro;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    static ResourcesManager instance;
    public static ResourcesManager Instance { get; private set; }
    
    private TextMeshProUGUI energieUI;
    private TextMeshProUGUI geldUI;
    private TextMeshProUGUI stressUI;

    private int energie = 0;
    private float geld = 0;
    private int stress = 0;
    public int Energie { 
        get { return energie; } 
        set
        {
            energie = value;
            energieUI.text = $"Energie: {energie}";
        } 
    }
    public float Geld
    {
        get { return geld; }
        set
        {
            geld = (Mathf.Floor(value * 100))*0.01f;    
            geldUI.text = $"Geld: ${geld}";
        }
    }
    public int Stress
    {
        get { return stress; }
        set
        {
            stress = value;
            stressUI.text = $"Stress: {stress}";
        }
    }

    private void Awake()
    {
        // If instance exists and is not this one, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        // Else make this the instance and find text objects
        else
        {
            instance = this;
            energieUI = transform.Find("Energie").GetComponent<TextMeshProUGUI>();
            geldUI = transform.Find("Geld").GetComponent<TextMeshProUGUI>();
            stressUI = transform.Find("Stress").GetComponent<TextMeshProUGUI>();
        }
        Energie = 100;
        Geld += 2.56001f;
        Stress = 4;
    }
    
}
