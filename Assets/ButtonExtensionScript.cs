using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExtensionScript : MonoBehaviour
{
    public Sprite[] sprites;
    Button b;
    Image i;

    bool pressed;

    // Start is called before the first frame update
    void Start()
    {
        b = GetComponentInParent<Button>();
        print(b.name);
        b.onClick.AddListener(Clicked);
        i = GetComponent<Image>();
    }

    void Clicked()
    {
        print("Clicked");
        pressed = true;
        i.sprite = sprites[1];
    }
    private void Update()
    {
        if(Input.GetMouseButtonUp(0) && pressed)
        {
            pressed = false;
            //i.sprite = sprites[0];
        }
    }
}
