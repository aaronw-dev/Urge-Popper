using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class LoadingBar : MonoBehaviour
{
    [Range(0, 1)]
    public float fillAmount;
    public float barSpeed = 5;
    public float rightTextMargin = 10;

    public Image loadingBar;
    public TMP_Text loadingBarText;
    RectTransform rt;
    float rightOffset;
    void Start()
    {
        LoadSceneManager.Instance.OnProgressChanged += RecalculateFill;
        rt = loadingBarText.rectTransform;
    }
    public void RecalculateFill(float progress)
    {
        fillAmount = progress;
        loadingBarText.text = MathF.Round(progress * 100, 1).ToString() + "%";
    }
    void Update()
    {
        rightOffset = Mathf.Lerp(rt.offsetMax.x, -(Camera.main.pixelWidth * (1 - fillAmount)) - rightTextMargin, barSpeed * Time.deltaTime);
        loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, fillAmount, barSpeed * Time.deltaTime);
        rt.offsetMax = new Vector2(rightOffset, rt.offsetMax.y);
    }
}