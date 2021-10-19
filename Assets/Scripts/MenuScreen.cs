using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public enum TransitionType
    {
        buttonLerp,
        fadeToBlack
        
    };

    [System.Serializable]
    public class ScreenTransitionInfo
    {
        public MenuScreen screenToMoveTo;
    }

    public MenuScreen[] targetScreens;
    [HideInInspector]
    public RectTransform rectTransform;
    TitleScreenManager tsm;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        tsm = FindObjectOfType<TitleScreenManager>();
    }

    public void ButtonWasPressed(int button)
    {
        MenuScreen s = targetScreens[button];
        StartCoroutine(tsm.MoveToScreen(this, s));
    }
}
