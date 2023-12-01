using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class LeaderboardObject
{
    public int prepid;
    public int prepamt;
}
public class LeaderboardManager : MonoBehaviour
{
    string apiUrl = "https://big-balls-leaderboard.aw-dev.repl.co/leaderboard/main";
    IEnumerator GetIdentification()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                apiid = request.downloadHandler.text;
                JsonUtility.FromJson<itemprep>(jsonst);
            }
        }
    }
}
