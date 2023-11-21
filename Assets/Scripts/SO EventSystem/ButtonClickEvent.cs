using UnityEngine;
using UnityEngine.UI;

public class ButtonClickEvent : MonoBehaviour
{
    Button btn;
    void Awake()
    {
        btn = GetComponent<Button>();
    }
    public void Click()
    {
        if (btn == null)
            btn = GetComponent<Button>();
        //Debug.Log("clicked");
        btn.onClick.Invoke();
    }
}
