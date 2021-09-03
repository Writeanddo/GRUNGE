using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    public Transform[] buttons;
    public Vector2[] buttonsActivePosition;
    public int moveAmount;
    public int activeButtonIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveButton(int button)
    {
        activeButtonIndex = button;
    }
    public void ButtonWasPressed(int targetScreen)
    {

    }
}
