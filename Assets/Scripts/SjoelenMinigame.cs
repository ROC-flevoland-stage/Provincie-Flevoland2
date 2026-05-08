using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SjoelenMinigame : MonoBehaviour
{
    // gamestates enum
    private enum gameStates
    {
        Aiming,
        Menu,
        Shoot
    }
    //shitty singleton implementation my brain is so foggy oml
    public SjoelenMinigame()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public static SjoelenMinigame Instance { 
        get {
            if (instance != null)
            {
                return instance;
            } 
            else
            {
                instance = new SjoelenMinigame();
                return instance;
            }
            
        } private set { 
            instance = value; 
        } 
    }
    private static SjoelenMinigame instance;

    // variables declarations
    private gameStates gameState = gameStates.Aiming;

    //gameobjects
    [SerializeField]
    private GameObject aimGuide;
    [SerializeField]
    private GameObject puckPrefab;
    [SerializeField]
    private Vector3 puckSpawnOffset;
    [SerializeField]
    private float puckShootSpeed;
    void Start()
    {

    }
    void Update()
    {
        switch (gameState)
        {
            case gameStates.Aiming:
                AimingBehavior();
                break;
            case gameStates.Shoot:
                ShootBehavior();
                break;
            case gameStates.Menu:
                MenuBehavior();
                break;

            default:
                Debug.Log("no gamestate");
                break;
        }
    }
    public void PuckTrigger(int trigger)
    {
        Debug.Log("Received puck trigger with number " + trigger);
        gameState = gameStates.Aiming;
    }

    private void AimingBehavior()
    {
        if (!Input.GetKey(KeyCode.LeftControl)) //If user is not holding L-ctrl
        {
            Cursor.lockState = CursorLockMode.Locked; // Keep cursor locked
            aimGuide.transform.Rotate(0,Input.mousePositionDelta.x,0); //Rotate aimguide according to mouse movement
        }
        else
        {
            Debug.Log("holding ctrl");
            Cursor.lockState = CursorLockMode.None; // Unlock cursor
        }
        Debug.Log(aimGuide.transform.rotation);
        if (Input.GetMouseButtonDown(0)) //When left clicked
        {
            //shooting puck code
            Quaternion rotation = aimGuide.transform.rotation;
            rotation *= Quaternion.Euler(0, 0, 0);

            GameObject puck = Instantiate(puckPrefab,aimGuide.transform.position+puckSpawnOffset,aimGuide.transform.rotation);
            //puck.GetComponent<Rigidbody>().linearVelocity =
            //    rotation * (new Vector3(puckShootSpeed,0,0));

            puck.GetComponent<Rigidbody>().AddForce(rotation * (new Vector3(puckShootSpeed * 50, 0, 0)), ForceMode.Force);

            Cursor.lockState = CursorLockMode.None; //Unlock cursor
            gameState = gameStates.Shoot; // Change gamestate
            Debug.Log("shot puck");
        }
        
    }
    private void ShootBehavior()
    {

    }
    private void MenuBehavior()
    {

    }
}
