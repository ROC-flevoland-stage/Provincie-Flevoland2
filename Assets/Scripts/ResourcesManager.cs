using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour
{
    static ResourcesManager instance; // instance of this object
    public static ResourcesManager Instance { get; private set; } //get set voor instance

    private TextMeshProUGUI geldTextValue; //ui object for geld text 
    private Slider energieSliderValue; //ui object for energie slider value
    private Slider stressSliderValue; //ui object for stress slider value

    [SerializeField]
    private float geld = 0; //float that internally stores geld
    [SerializeField]
    private int energie = 0; //int that internally stores energie
    [SerializeField]
    private int maxEnergie; //int that internally stores max energie
    [SerializeField]
    private int stress = 0; //int that internally stores stress
    [SerializeField]
    private int maxStress; //int that internally stores max stress



    // get set for Geld
    [SerializeField]
    public float Geld
    {
        get { return geld; }
        set
        {
            geld = (Mathf.Floor(value * 100)) * 0.01f; // Set internal value to given value, rounding it to cents
            SaveManager.CreateOrSetValue<float>("Resource_Manager_Geld", value, true); // Set value in save file
            geldTextValue.text = geld.ToString(); // Set text value
        }
    }

    // get set for Energie
    public int Energie { 
        get { return energie; } 
        set
        {
            if (value > maxEnergie) { value = maxEnergie; } // If given value exceeds max, clamp
            energie = value; // Set internal value to given value
            SaveManager.CreateOrSetValue<int>("Resource_Manager_Energie", value, true); // Set value in save file
            energieSliderValue.value = (float)energie/maxEnergie; // Set slider value
        } 
    }

    // get set for Stress
    public int Stress
    {
        get { return stress; }
        set
        {
            if(value > maxStress) { value = maxStress; } // If given value exceeds max, clamp
            stress = value; // Set internal value to given value
            SaveManager.CreateOrSetValue<float>("Resource_Manager_Stress", value, true); // Set value in save file
            stressSliderValue.value = (float)stress / maxStress; // Set slider value
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

            // Find UI components
            geldTextValue = transform.Find("Geld").GetComponent<TextMeshProUGUI>();
            energieSliderValue = transform.Find("Energie").GetComponent<Slider>();
            stressSliderValue = transform.Find("Stress").GetComponent<Slider>();


            // TEMPORARY load save file
            SaveManager.LoadDataFromFile();
            // Get values from save file
            float _geld;
            int _energie;
            int _stress;
            if (SaveManager.TryGetValue<float>("Resource_Manager_Geld", out _geld))
            {
                Geld = _geld;
            }
            if (SaveManager.TryGetValue<int>("Resource_Manager_Energie", out _energie))
            {
                Energie = _energie;
            }
            if (SaveManager.TryGetValue<int>("Resource_Manager_Stress", out _stress))
            {
                Stress = _stress;
            }

            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        maxEnergie = (maxEnergie == 0) ? 1 : maxEnergie; // If max is for whatever reason 0, set it to 1 to avoid divide by 0
        maxStress = (maxStress == 0) ? 1 : maxStress; // Same as above
    }

}
