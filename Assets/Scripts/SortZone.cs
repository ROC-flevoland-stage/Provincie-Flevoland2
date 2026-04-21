using UnityEngine;

public class SortZone : MonoBehaviour
{
    public FileType acceptsType;
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        File file = other.GetComponent<File>();
        if (file == null) return;

        if (file.fileType == acceptsType)
        {
            gameManager.CorrectFile(other.gameObject);
        }
        else
        {
            gameManager.WrongFile(other.gameObject);
        }
    }
}
