using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance;
    public delegate void ProgressChange(float progress);
    public event ProgressChange OnProgressChanged;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }
    public void LoadScene(string SceneName)
    {
        StartCoroutine(LoadSceneAsynchronous(SceneName));
    }
    IEnumerator LoadSceneAsynchronous(string SceneName)
    {
        yield return null;
        AsyncOperation load = SceneManager.LoadSceneAsync(SceneName);
        while (!load.isDone)
        {
            OnProgressChanged.Invoke(load.progress);
            yield return null;
        }
    }
    public void LoadScene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
}
