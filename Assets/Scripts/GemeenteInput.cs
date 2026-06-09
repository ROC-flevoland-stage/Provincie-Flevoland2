using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GemeenteInput : MonoBehaviour
{
    public Dropdown gemeenteDropdown;
    public Text gemeenteText;
    public CharacterData characterData;
    
    //Dit is voor de gemeente dropdown, dit wordt opgeslagen in characterData
    public void OnGemeenteChanged(int index)
    {
        if (gemeenteDropdown == null) return;

        string selectedGemeente = gemeenteDropdown.options[index].text;
        SaveManager.CreateOrSetValue("Achtergrond_Gemeente", selectedGemeente, true);
        if (gemeenteText != null)
            gemeenteText.text = "Gemeente: " + selectedGemeente;

        if (characterData != null)
            characterData.municipality = selectedGemeente;
    }
}