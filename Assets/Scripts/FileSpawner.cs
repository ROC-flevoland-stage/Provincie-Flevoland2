using UnityEngine;
using TMPro;

public class FileSpawner : MonoBehaviour
{
    public GameObject[] cubesToSpawn;
    public string[] spawnLabels; // edit these in the Inspector to change text per cube
    public GameManager gameManager;

    [Header("UI")]
    public TMP_Text spawnIndicatorText; // assign a TextMeshProUGUI element in the Canvas (top middle)

    GameObject currentInstance;
    int spawnIndex;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        spawnIndex = 0;
        TrySpawn();
    }

    void Update()
    {
        if (currentInstance == null)
            TrySpawn();
    }

    void TrySpawn()
    {
        if (spawnIndex >= cubesToSpawn.Length) return;
        var prefab = cubesToSpawn[spawnIndex];
        if (prefab == null) return;
        if (gameManager != null && spawnIndex >= gameManager.totalCubes) return;

        // spawned op spawner
        currentInstance = Instantiate(prefab, transform.position, Quaternion.identity);

        // kiest de label
        string label = null;
        if (spawnLabels != null && spawnLabels.Length > spawnIndex && !string.IsNullOrEmpty(spawnLabels[spawnIndex]))
            label = spawnLabels[spawnIndex];

        var fileComp = currentInstance.GetComponent<SpawnedFile>();
        if (fileComp != null)
        {
            fileComp.index = spawnIndex;
            if (string.IsNullOrEmpty(label))
            {
                // bestaande label blijft zolang er iets staat anders default het naar item
                label = !string.IsNullOrEmpty(fileComp.label) ? fileComp.label : "Item " + (spawnIndex + 1);
            }
            fileComp.label = label;
        }
        else
        {
            if (string.IsNullOrEmpty(label))
                label = "Item " + (spawnIndex + 1);
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

        spawnIndex++;
    }
}
