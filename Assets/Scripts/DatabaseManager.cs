using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class DataBaseManager : MonoBehaviour
{
    private string uri = "http://localhost:8000/index.php";


    /// <summary>
    /// Function to upload save file to database
    /// </summary>
    /// <param name="json">json file to upload</param>
    public void UploadSaveFile(string json) 
    {
        StartCoroutine(upload(json));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    private IEnumerator upload(string json)
    {
        UnityWebRequest www = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}