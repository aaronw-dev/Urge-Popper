using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextmeshproEventInvoker : MonoBehaviour
{

    TextMeshProUGUI m_text;
    public string m_prefix;
    public string m_suffix;

    [Header("PlayerPrefs")]
    [ShowIf("isPlayerPrefs")] public string m_prefKey;
    [ShowIf("isPlayerPrefs")] public PlayerPrefsDataType playerPrefsDataType;
    public Parameters m_parameterToDisplay;
    /*    [ShowIf("isOther")] public ScriptableObject gameEvent;*/
    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        OnEnable();
    }
    bool isPlayerPrefs() { return m_parameterToDisplay == TextmeshproEventInvoker.Parameters.PLAYERPREFS; }
    private void OnEnable()
    {
        switch (m_parameterToDisplay)
        {
            case Parameters.NONE:
                break;
            case Parameters.PLAYERPREFS:
                switch (playerPrefsDataType)
                {
                    case PlayerPrefsDataType.FLOAT:
                        Invoke(PlayerPrefs.GetFloat(m_prefKey));
                        break;

                    case PlayerPrefsDataType.STRING:
                        InvokeStr(PlayerPrefs.GetString(m_prefKey));
                        break;

                    case PlayerPrefsDataType.INT:
                        Invoke(PlayerPrefs.GetInt(m_prefKey));
                        break;

                }
                break;
            default:
                break;
        }
    }
    public void Invoke(int _i)
    {
        CheckTxtObj();
        m_text.text = m_prefix + ": " + _i.ToString() + m_suffix;
    }
    public void InvokeStr(string _str)
    {
        CheckTxtObj();
        m_text.text = (!m_prefix.Equals("") ? m_prefix + ": " : m_prefix) + _str.ToString() + m_suffix;
    }
    public void InvokeLocalizedStr(string _str)
    {
        CheckTxtObj();
        m_text.text = m_prefix + _str.ToString() + m_suffix;
    }
    public void Invoke(float _f)
    {
        CheckTxtObj();
        m_text.text = m_prefix + _f.ToString() + m_suffix;
    }
    public void CheckTxtObj()
    {
        if (!m_text)
            m_text = GetComponent<TextMeshProUGUI>();
    }
    public enum PlayerPrefsDataType
    {
        FLOAT, STRING, INT
    }
    public enum Parameters
    {

        NONE,

        PLAYERPREFS,

    }
}
