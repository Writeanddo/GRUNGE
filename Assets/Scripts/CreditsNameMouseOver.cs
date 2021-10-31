using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CreditsNameMouseOver : MonoBehaviour
{
    public Animator associatedIcon;
    TitleScreenManager tsm;
    TextMeshProUGUI text;
    AudioSource sfx;
    bool mouseOver;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        tsm = FindObjectOfType<TitleScreenManager>();
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        mouseOver = true;
        associatedIcon.Play("DevHeadBounce", -1, 0);
        text.color = Color.green;
        int rand = Random.Range(0, 3);
        sfx.PlayOneShot(tsm.uiSfx[rand]);

    }

    private void OnMouseExit()
    {
        text.color = Color.white;
        associatedIcon.Play("DevHeadIdle", -1, 0);
        mouseOver = false;
    }
}
