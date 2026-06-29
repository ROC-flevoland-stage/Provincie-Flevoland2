using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.Multiplayer.Center.Common;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    private GameObject QuestionVerificationPrefab;

    //Question Verification
    [SerializeField]
    private int QuestionVerificationPosition;
    [SerializeField]
    private int QuestionVerificationOffset;

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
    public string[] Questions;
    [SerializeField]
    public string[] QuestionsID;
    private int curQuestion;
    private int[] antwoorden;


    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Questions.Length != QuestionsID.Length)
        {
            Debug.LogError("Questions length does not match QuestionsID length");
        }
        curQuestion = 0;
        questionText.GetComponent<TextMeshProUGUI>().text = Questions[curQuestion];
        antwoorden = new int[Questions.Length];
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
        if(curQuestion >= Questions.Length)
        {
            finished = true;
            EndGame();  
        }
        else
        {
            questionText.GetComponent<TextMeshProUGUI>().text = Questions[curQuestion];
            gameState = gameStates.Aiming;
        }
    }

    private void AimingBehavior()
    {
        puckShootDelayTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PuckTrigger(1);
            Cursor.lockState = CursorLockMode.None; //Unlock cursor
        } else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PuckTrigger(2);
            Cursor.lockState = CursorLockMode.None; //Unlock cursor
        } else
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PuckTrigger(3);
            Cursor.lockState = CursorLockMode.None; //Unlock cursor
        } else
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PuckTrigger(4);
            Cursor.lockState = CursorLockMode.None; //Unlock cursor
        } else
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PuckTrigger(5);
            Cursor.lockState = CursorLockMode.None; //Unlock cursor
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
        Cursor.lockState = CursorLockMode.None; //Unlock cursor
        if (!finished) 
        {
            
        } else
        {

        }
    }
    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        gameState = gameStates.Aiming;
        StartScreen.SetActive(false);
        GameOverlay.SetActive(true);

    }
    public void EndGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameState = gameStates.Menu;
        EndScreen.SetActive(true);
        GameOverlay.SetActive(false);

        Transform endscreenTransform = EndScreen.transform;
        for (int i = 0; i < Questions.Length; i++)
        {
            GameObject QuestionVerification = Instantiate(QuestionVerificationPrefab, endscreenTransform);
            QuestionVerification.GetComponent<SjoelenQuestion>().QuestionIndex = i;
            QuestionVerification.transform.Translate(new Vector3(0, QuestionVerificationPosition + i * -QuestionVerificationOffset, 0));
        }
    }

    public void ExitMinigame()
    {
        for(int i = 0; i < Questions.Length; i++)
        {
            SaveManager.CreateOrSetValue(QuestionsID[i], antwoorden[i], true);            
        }
        SceneManager.LoadScene("E3Demo");
    }

    /// <summary>
    /// Tries to change a previous given answer and returns the new value
    /// </summary>
    /// <param name="answerIndex"></param>
    /// <param name="changeAmount"></param>
    public int TryChangeAnswer(int answerIndex, int changeAmount)
    {
        return (antwoorden[answerIndex] = Mathf.Clamp((antwoorden[answerIndex] + changeAmount),1,5));
    }
}
