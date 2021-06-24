using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour
{
    public class DialogLines
    {
        public string[] lines;
    }

    public string[] testLines;
    public Sprite[] testPortraitFrames;

    GameManager gm;

    Text dialogText;
    Image dialogPortrait;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        dialogPortrait = transform.GetChild(0).GetComponent<Image>();
        dialogText = transform.GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(PrintText(testLines));

    }

    public IEnumerator PrintText(string[] lines)
    {
        for(int i = 0; i < lines.Length; i++)
        {
            string line = InsertLineBreaks(lines[i]);
            for(int j = 0; j < lines[i].Length; j++)
            {
                if(j % 4 == 0)
                {
                    dialogPortrait.sprite = testPortraitFrames[1];
                    gm.PlaySFXStoppable(gm.generalSfx[11], Random.Range(0.8f, 1.2f));
                }
                else if(j % 2 == 0)
                    dialogPortrait.sprite = testPortraitFrames[0];

                dialogText.text = line.Substring(0, j+1);
                yield return new WaitForSeconds(0.025f);
            }
            dialogText.text = line;
        }
        dialogPortrait.sprite = testPortraitFrames[0];
    }

    string InsertLineBreaks(string s)
    {
        string t = "";

        for (int i = 0; i < s.Length; i++)
        {
            // Add line break
            if (s[i] == '\\')
                t += "\n";
            else
                t += s[i];
        }
        return t;
    }
}
