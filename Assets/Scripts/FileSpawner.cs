using UnityEngine;
using TMPro;

public class FileSpawner : MonoBehaviour
{
    public static FileSpawner Instance { get { return _instance; } }
    private static FileSpawner _instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    public GameObject[] cubesToSpawn;
    public string[] spawnLabels; // edit these in the Inspector to change text per cube
    public string[] spawnLabelsIDs;
    public int[] answers;
    public GameManager gameManager;

    [Header("UI")]
    public TMP_Text spawnIndicatorText; // assign a TextMeshProUGUI element in the Canvas (top middle)

    GameObject currentInstance;
    int questionIndex;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        questionIndex = 0;
        TrySpawn();
    }

    void Update()
    {
        if (currentInstance == null)
            TrySpawn();
    }

    void TrySpawn()
    {
        if (questionIndex >= cubesToSpawn.Length) return;
        var prefab = cubesToSpawn[questionIndex];
        if (prefab == null) return;
        if (gameManager != null && questionIndex >= gameManager.totalCubes) return;

        // spawned op spawner
        currentInstance = Instantiate(prefab, transform.position, Quaternion.identity);

        // kiest de label
        string label = null;
        if (spawnLabels != null && spawnLabels.Length > questionIndex && !string.IsNullOrEmpty(spawnLabels[questionIndex]))
            label = spawnLabels[questionIndex];

        var fileComp = currentInstance.GetComponent<SpawnedFile>();
        if (fileComp != null)
        {
            fileComp.index = questionIndex;
            if (string.IsNullOrEmpty(label))
            {
                // bestaande label blijft zolang er iets staat anders default het naar item
                label = !string.IsNullOrEmpty(fileComp.label) ? fileComp.label : "Item " + (questionIndex + 1);
            }
            fileComp.label = label;
        }
        else
        {
            if (string.IsNullOrEmpty(label))
                label = "Item " + (questionIndex + 1);
        }

        // If prefab contains a TextMeshPro (3D) element, set it
        var tm3 = currentInstance.GetComponentInChildren<TMP_Text>();
        if (tm3 != null)
            tm3.text = label;

        // Update top-middle UI indicator with the same label
        if (spawnIndicatorText != null)
        {
            spawnIndicatorText.gameObject.SetActive(true);
            spawnIndicatorText.text = label;
        }

        questionIndex++;
    }
}
