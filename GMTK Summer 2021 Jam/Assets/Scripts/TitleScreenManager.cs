using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public RectTransform[] menuScreens;
    int activeMenuScreen = 0;
    // 0: top menu
    // 1: level select
    // 2: options
    // 3: credits
    // 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void SetMenuScreen(int level)
    {
        activeMenuScreen = level;
        for(int i = 0; i < menuScreens.Length; i++)
        {
            if (i == activeMenuScreen)
            {
                menuScreens[i].anchoredPosition = Vector2.zero;
            }
            else
                menuScreens[i].anchoredPosition = new Vector2(0, -1500);
        }
    }

    public IEnumerator LoadLevel()
    {
        yield return null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
