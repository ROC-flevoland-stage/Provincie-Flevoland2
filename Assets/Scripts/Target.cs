using System.Reflection;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int keuzeNummer = 1; // per target assignen in de inspector (1-10)

    // Script runt nogsteeds als Target, maar we willen niet afhankelijk zijn van de exacte class name of method name in QuestionsManager
    object questionsManagerInstance;
    MethodInfo choiceMethod;

    void Start()
    {
        // vind de QuestionsManager instance door te zoeken naar een MonoBehaviour met de juiste methoden, ongeacht de class name
        var all = FindObjectsOfType<MonoBehaviour>();
        foreach (var mb in all)
        {
            var t = mb.GetType();
            if (t.Name == "QuestionsManager")
            {
                questionsManagerInstance = mb;
                // ivm problemen bestaat deze script. Als er een method is genaamd "ChoiceSelected" of "SelectChoice" nemen we die, anders niet
                choiceMethod = t.GetMethod("ChoiceSelected", BindingFlags.Instance | BindingFlags.Public)
                             ?? t.GetMethod("SelectChoice", BindingFlags.Instance | BindingFlags.Public);
                break;
            }
        }
        // Log warnings als we geen QuestionsManager vinden of als die geen geschikte method heeft
        if (questionsManagerInstance == null)
            Debug.LogWarning("Target: QuestionsManager instance not found in scene.");
        else if (choiceMethod == null)
            Debug.LogWarning($"Target: QuestionsManager found but no ChoiceSelected/SelectChoice method on type {questionsManagerInstance.GetType().FullName}.");
    }

    void OnMouseDown()
    {
        if (questionsManagerInstance != null && choiceMethod != null)
        {
            choiceMethod.Invoke(questionsManagerInstance, new object[] { keuzeNummer });
        }
        else
        {
            Debug.Log("Keuze " + keuzeNummer);
        }
    }
}
