using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsIconMouseOver : MonoBehaviour
{
    public TextMeshProUGUI[] associatedTexts;
    Animator associatedIcon;
    TitleScreenManager tsm;
    AudioSource sfx;
    bool mouseOver;

    // Start is called before the first frame update
    void Start()
    {
        associatedIcon = GetComponent<Animator>();
        tsm = FindObjectOfType<TitleScreenManager>();
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        print("Entered");
        mouseOver = true;
        associatedIcon.Play("DevHeadBounce", -1, 0);
        for(int i = 0; i < associatedTexts.Length; i++)
            associatedTexts[i].color = Color.green;
        int rand = Random.Range(0, 3);
        sfx.PlayOneShot(tsm.uiSfx[rand]);

    }

    private void OnMouseExit()
    {
        for (int i = 0; i < associatedTexts.Length; i++)
            associatedTexts[i].color = Color.green;
        associatedIcon.Play("DevHeadIdle", -1, 0);
        mouseOver = false;
    }
}
