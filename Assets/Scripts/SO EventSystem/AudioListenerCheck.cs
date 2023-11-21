using UnityEngine;

public class AudioListenerCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetInt("AudioOn", 1);
    }

}
