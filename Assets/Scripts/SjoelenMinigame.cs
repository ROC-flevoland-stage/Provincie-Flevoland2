using System.Collections;
using TMPro;
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
    private gameStates gameState = gameStates.Menu;
    private bool finished = false;

    //gameobjects
    [SerializeField]
    private GameObject aimGuide;
    [SerializeField]
    private GameObject puckPrefab;
    [SerializeField]
    private GameObject questionText;
    [SerializeField] 
    private GameObject GameOverlay;
    [SerializeField]
    private GameObject StartScreen;
    [SerializeField]
    private GameObject EndScreen;

    //puck spawning
    [SerializeField]
    private Vector3 puckSpawnOffset;
    [SerializeField]
    private float puckShootSpeed;
    [SerializeField]
    private float puckShootDelay;
    private float puckShootDelayTimer = 0;

    //save data collection
    [SerializeField]
    private string[] questions;
    private int curQuestion;
    private int[] antwoorden;


    void Start()
    {
        curQuestion = 0;
        questionText.GetComponent<TextMeshProUGUI>().text = questions[curQuestion];
        antwoorden = new int[questions.Length];
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
    //Called when puck enters trigger
    public void PuckTrigger(int trigger)
    {
        Debug.Log("Received puck trigger with number " + trigger);
        antwoorden[curQuestion] = trigger;
        curQuestion += 1;
        if(curQuestion >= questions.Length)
        {
            finished = true;
            EndGame();  
        }
        else
        {
            questionText.GetComponent<TextMeshProUGUI>().text = questions[curQuestion];
            gameState = gameStates.Aiming;
        }
    }

    private void AimingBehavior()
    {
        puckShootDelayTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1)) { 
            PuckTrigger(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PuckTrigger(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PuckTrigger(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PuckTrigger(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PuckTrigger(5);
        }

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
        if (Input.GetMouseButtonDown(0) && puckShootDelayTimer <= 0) //When left clicked
        {
            //shooting puck code
            Quaternion rotation = aimGuide.transform.rotation;
            rotation *= Quaternion.Euler(0, 0, 0);

            GameObject puck = Instantiate(puckPrefab,aimGuide.transform.position+puckSpawnOffset,aimGuide.transform.rotation);
            //puck.GetComponent<Rigidbody>().linearVelocity =
            //    rotation * (new Vector3(puckShootSpeed,0,0));

            puck.GetComponent<Rigidbody>().AddForce(rotation * (new Vector3(puckShootSpeed * 50, 0, 0)), ForceMode.Force);

            Cursor.lockState = CursorLockMode.None; //Unlock cursor
            //gameState = gameStates.Shoot; // Change gamestate
            Debug.Log("shot puck");
            puckShootDelayTimer = puckShootDelay;
        }
        
    }
    private void ShootBehavior()
    {

    }
    private void MenuBehavior()
    {
        if (!finished) 
        {
            
        } else
        {

        }
    }
    public void StartGame()
    {
        gameState = gameStates.Aiming;
        StartScreen.SetActive(false);
        GameOverlay.SetActive(true);

    }
    public void EndGame()
    {
        gameState = gameStates.Menu;
        EndScreen.SetActive(true );
        GameOverlay.SetActive(false);
    }
}
