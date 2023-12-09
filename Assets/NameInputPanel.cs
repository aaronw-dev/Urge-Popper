using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using MoreMountains.Tools;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class NameInputPanel : MonoBehaviour
{
    public TMP_InputField nameInput;
    string IDUrl = "https://big-balls-leaderboard.aw-dev.repl.co/getid";
    string apiUrl = "https://big-balls-leaderboard.aw-dev.repl.co/uploadscore";
    GameObject container;

    [Space]
    public GameObject Animationcontainer;
    public GameObject AnimationcontainerButton;
    public TextMeshProUGUI animationTxt;
    void Start()
    {
        container = transform.GetChild(0).gameObject;
        Client.ActiveClient.fetchInformation();
    }
    string randomPrefix;
    string _name;
    public void Submit()
    {
        randomPrefix = NamePrefixes.prefixes[Random.Range(0, NamePrefixes.prefixes.Length)];
        string randomSuffix = NamePrefixes.suffixes[Random.Range(0, NamePrefixes.suffixes.Length)];
        _name = nameInput.text.ToTitleCase();
        string combinedName = randomPrefix + " " + _name;
        PlayerPrefs.SetString("_name", combinedName);
        PlayerPrefs.Save();
        Client.ActiveClient.username = combinedName;
        StartCoroutine(CreateUserProfile());
        StartCoroutine(Animation());
    }
    public bool doneFetching = false;
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    IEnumerator Animation()
    {
        container.SetActive(false);
        Animationcontainer.SetActive(true);
        string rand = "";
        string newRand = "";
        int revealCounter = 0;
        int FakeCharracter = 0;
        while (revealCounter < randomPrefix.Length - 1)
        {
            for (int i = 0; i < 5; i++)
            {
                rand = new string(Enumerable.Repeat(chars, Mathf.Clamp(randomPrefix.Length - FakeCharracter, 0, int.MaxValue)).Select(s => s[Random.Range(0, s.Length)]).ToArray());

                newRand = new string(Enumerable.Repeat(chars, FakeCharracter - revealCounter).Select(s => s[Random.Range(0, s.Length)]).ToArray());
                animationTxt.text = randomPrefix.Substring(0, revealCounter) + newRand + "<alpha=#88>" + rand + "<alpha=#FF> " + _name;

                yield return new WaitForSeconds(0.075f);
            }
            if (doneFetching)
            {
                FakeCharracter++;
                revealCounter = Mathf.Clamp(FakeCharracter - 3, 0, int.MaxValue);
            }
        }
        animationTxt.text = randomPrefix + " " + _name;
        animationTxt.transform.DOPunchScale(Vector3.one * 0.125f, 0.25f);
        yield return new WaitForSeconds(0.45f);
        AnimationcontainerButton.SetActive(true);

    }
    [Button]
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    IEnumerator CreateUserProfile()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(IDUrl))
        {
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Client.ActiveClient.id = request.downloadHandler.text;
                PlayerPrefs.SetString("_id", Client.ActiveClient.id);
                PlayerPrefs.Save();
            }
        }
        yield return StartCoroutine(Client.ActiveClient.fetchInformation());
        string username = Client.ActiveClient.username;
        string id = Client.ActiveClient.id;
        string league = Client.ActiveClient.league = "nutter";
        string json = "{\"username\":\"" + username + "\", \"league\":\"" + league + "\", \"score\":" + 0 + ", \"id\":\"" + id + "\",\"country\":\"" + Client.ActiveClient.countryCode + "\"}";
        Debug.Log(json);
        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, json.ToString()))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                yield return new WaitForSeconds(2.25f);
                doneFetching = true;
                yield break;
            }
        }
        yield return new WaitForSeconds(2.25f);
        doneFetching = true;

        StartCoroutine(Client.ActiveClient.fetchInformation());
    }
}
