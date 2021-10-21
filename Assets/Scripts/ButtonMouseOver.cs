using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMouseOver : MonoBehaviour
{
    public int offset = 64;

    Vector2 startPos;
    RectTransform t;
    bool mouseOver;
    AudioSource sfx;
    TitleScreenManager tsm;
    Button b;

    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<Button>();
        tsm = FindObjectOfType<TitleScreenManager>();
        t = GetComponent<RectTransform>();
        startPos = t.anchoredPosition;
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 target = Vector2.zero;
        if (mouseOver)
            target = new Vector2(offset, 0);

        t.anchoredPosition = Vector2.Lerp(t.anchoredPosition, startPos + target, 0.5f);
        if (tsm.performingScreenTransition)
            mouseOver = false;
    }

    private void OnMouseEnter()
    {
        if (b.interactable && !tsm.performingScreenTransition)
        {
            mouseOver = true;
            int rand = Random.Range(0, 3);
            sfx.PlayOneShot(tsm.uiSfx[rand]);
        }
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }
}
