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
        public int nextScreenDepth;
        public int nextScreenRemainingButton;
        public MenuScreen screenToMoveTo;
        public TransitionType transitionType;
    }

    public Button[] buttons;
    public ScreenTransitionInfo[] transitions;

    [HideInInspector]
    public Vector2[] buttonsActivePosition;
    public int moveAmount;
    public int activeButtonIndex;

    TitleScreenManager tsm;

    // Start is called before the first frame update
    void Start()
    {
        tsm = FindObjectOfType<TitleScreenManager>();

        buttonsActivePosition = new Vector2[buttons.Length];
        for (int i = 0; i < buttonsActivePosition.Length; i++)
            buttonsActivePosition[i] = buttons[i].GetComponent<RectTransform>().anchoredPosition;
    }

    public void ButtonWasPressed(int button)
    {
        activeButtonIndex = button;
        ScreenTransitionInfo s = transitions[button];
        if (s.transitionType == TransitionType.buttonLerp)
            StartCoroutine(tsm.MenuScreenButtonTransition(s.nextScreenDepth, s.nextScreenRemainingButton, this, s.screenToMoveTo));
    }
}
