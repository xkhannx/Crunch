using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] RectTransform levelsPanel;
    [SerializeField] RectTransform gamePanel;
    
    public void LoadGame(Action callBack)
    {
        StartCoroutine(LevelsPanelFlyOut(callBack));
    }

    IEnumerator LevelsPanelFlyOut(Action callBack)
    {
        float h = Screen.height;
        float flyTime = 0.5f;
        float t = 0;

        while (t < flyTime)
        {
            t += Time.deltaTime;

            levelsPanel.anchoredPosition = Vector2.up * t / flyTime * h;
            gamePanel.anchoredPosition = Vector2.up * (t / flyTime * h - h);
            yield return null;
        }
        
        callBack.Invoke();
    }
}
