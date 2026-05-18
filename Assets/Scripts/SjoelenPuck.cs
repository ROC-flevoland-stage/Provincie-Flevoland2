using Unity.VisualScripting;
using UnityEngine;

public class SjoelenPuck : MonoBehaviour
{
    private bool hasTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider) //Detect sjoelbak triggers
    {
        if (!hasTriggered) { 
            if(collider.tag == "sjoelbakTrigger")
            {
                string triggerNumberString = collider.name.Remove(0,6);
                bool parseSucces = int.TryParse(triggerNumberString, out int triggerNumber);
                if (parseSucces) {
                    SjoelenMinigame.Instance.PuckTrigger(triggerNumber);
                    hasTriggered = true;
                }
                else
                {
                    Debug.LogError("Couldn't parse int for sjoelbak trigger with name " + collider.name);
                }
            }
        }
    }
}
