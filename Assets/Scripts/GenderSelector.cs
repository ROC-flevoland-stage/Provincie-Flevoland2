using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GenderSelector : MonoBehaviour
{
    public Text genderText;

    public CharacterData characterData;

    // Speler select geslacht, dit wordt opgeslagen in characterData
    public void SelectMale()
    {
        genderText.text = "Geslacht: Man";

        if (characterData != null)
            characterData.gender = "Man";
        genderChanged();
    }
    public void SelectFemale()
    {
        genderText.text = "Geslacht: Vrouw";

        if (characterData != null)
            characterData.gender = "Vrouw";
        genderChanged();
    }

    public void SelectOther()
    {
        genderText.text = "Geslacht: Anders";

        if (characterData != null)
            characterData.gender = "Anders";
        genderChanged();
    }

    private void genderChanged()
    {
        SaveManager.CreateOrSetValue("Achtergrond_Geslacht", characterData.gender, true);
    }
}