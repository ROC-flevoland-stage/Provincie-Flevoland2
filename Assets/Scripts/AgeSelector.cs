using UnityEngine;
using UnityEngine.UI;

public class AgeSelector : MonoBehaviour
{
    public Slider ageSlider;
    public Text ageText;

    public CharacterData characterData;

    void Start()
    {

        UpdateAgeText(ageSlider.value);


        ageSlider.onValueChanged.AddListener(OnAgeChanged);
    }


    public void OnAgeChanged(float value)
    {
        UpdateAgeText(value);
        SaveManager.CreateOrSetValue("Achtergrond_Leeftijd",value,true);
    }
    // Update de leeftijdstekst en sla de waarde op in characterData
    private void UpdateAgeText(float value)
    {
        int age = Mathf.RoundToInt(value);
        ageText.text = "Leeftijd: " + age;

        if (characterData != null)
            characterData.age = age;
    }
}