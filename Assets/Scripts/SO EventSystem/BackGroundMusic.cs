using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackGroundMusic : MonoBehaviour
{
    public SceneBackgroundMusic[] _sceneBackgrounds;

    public static BackGroundMusic instance;
    private AudioSource[] _audioSources;
    public float fadeSpeed = 0.5f;
    public float fadeInOutMultiplier = 0.0f;
    public bool isPlaying;

    public string playingTrackName = "Nothing";
    public int playingTrackIndex;
    public float playingTrackVolume = 0.000f;

    public string lastTrackName = "Nothing";
    public int lastTrackIndex;
    public float lastTrackVolume = 0.000f;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _audioSources = GetComponentsInChildren<AudioSource>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        for (int i = 0; i < _sceneBackgrounds.Length; i++)
        {
            if (_sceneBackgrounds[i].index == scene.buildIndex)
            {

                PlayMusic(_sceneBackgrounds[i].source.name);

            }
        }


    }
    public void AffectSound(float volume)
    {
        if (FadingRoutine == null)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                _audioSources[i].DOFade(volume, .5f);
            }
        }
    }
    public IEnumerator FadeOutOldMusic_FadeInNewMusic()
    {
        _audioSources[playingTrackIndex].volume = 0.000f;
        _audioSources[playingTrackIndex].Play();
        while (_audioSources[playingTrackIndex].volume < 1f)
        {
            _audioSources[lastTrackIndex].DOFade(0, fadeSpeed).SetUpdate(true);
            _audioSources[playingTrackIndex].DOFade(1, fadeSpeed).SetUpdate(true);
            //Debug.Log("Fade: " + lastTrackName + " " + _audioSources[lastTrackIndex].volume.ToString() + " Rise: " + playingTrackName + " " + _audioSources[playingTrackIndex].volume.ToString());
            yield return new WaitForSecondsRealtime(fadeSpeed + 0.0001f);
            lastTrackVolume = _audioSources[lastTrackIndex].volume;
            playingTrackVolume = _audioSources[playingTrackIndex].volume;
        }
        _audioSources[lastTrackIndex].volume = 0.000f; // Just In Case....
        _audioSources[lastTrackIndex].Stop();

        lastTrackIndex = playingTrackIndex;
        lastTrackName = playingTrackName;
        isPlaying = true;
        FadingRoutine = null;

    }
    public IEnumerator FadeInNewMusic()
    {
        _audioSources[playingTrackIndex].volume = 0.000f;
        _audioSources[playingTrackIndex].Play();
        while (_audioSources[playingTrackIndex].volume < 1f)
        {
            _audioSources[playingTrackIndex].DOFade(1, fadeSpeed).SetUpdate(true);
            //Debug.Log("Fading In: " + _audioSources[track_index].volume.ToString());
            yield return new WaitForSecondsRealtime(fadeSpeed + 0.0001f);
            playingTrackVolume = _audioSources[playingTrackIndex].volume;
        }
        lastTrackIndex = playingTrackIndex;
        lastTrackName = playingTrackName;
        isPlaying = true;
        FadingRoutine = null;
    }
    Coroutine FadingRoutine;
    public void PlayMusic(string transformName)
    {
        for (int a = 0; a < _audioSources.Length; a++)
        {
            if (_audioSources[a].name == transformName)
            {
                Debug.Log("Found Track Name (" + transformName + ") at Index(" + a.ToString() + ")");
                playingTrackIndex = a;
                playingTrackName = transformName;
                if (FadingRoutine != null)
                    StopCoroutine(FadingRoutine);
                break;
            }
        }
        if (playingTrackIndex == lastTrackIndex)
        {
            Debug.Log("Same Track Selected");
            return;
        }
        else
        {
            if (isPlaying)
            {
                Debug.Log("Fading in new music - Fading out old music");
                FadingRoutine = StartCoroutine(FadeOutOldMusic_FadeInNewMusic());
            }
            else
            {
                Debug.Log("Fading in new music");
                FadingRoutine = StartCoroutine(FadeInNewMusic());
            }
        }
    }
}
[System.Serializable]
public class SceneBackgroundMusic
{
    public int index;
    public AudioSource source;
}