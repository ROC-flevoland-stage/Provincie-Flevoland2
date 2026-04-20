using UnityEngine;

public enum FileType
{ // De type files die we hebben. Rood word niet meer gebruikt maar ik laat het staan voor als ik er verder iets mee wil doen voor een andere minigame.
    Green,
    Red
}

public class File : MonoBehaviour
{
    public FileType fileType;
}
