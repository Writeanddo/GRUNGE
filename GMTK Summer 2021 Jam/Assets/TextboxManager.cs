using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour
{
    [System.Serializable]
    public class TextData
    {
        public string id; // Used so we can find next TextData in the event of multiple choices
        public string[] dialog;
        public int portraitIndex;
    }

    public TextData[] t;

    public string[] testLines;
    public Sprite[] portraitFrames;

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

    }

    public IEnumerator PrintAllText(TextData[] texts)
    {
        foreach (TextData t in texts)
        {
            for (int i = 0; i < t.dialog.Length; i++)
            {
                string line = InsertLineBreaks(t.dialog[i]);
                for (int j = 0; j < t.dialog[i].Length; j++)
                {
                    if (j % 4 == 0)
                    {
                        dialogPortrait.sprite = portraitFrames[t.portraitIndex + 1];
                        gm.PlaySFXStoppable(gm.generalSfx[11], Random.Range(0.5f, 0.8f));
                    }
                    else if (j % 2 == 0)
                        dialogPortrait.sprite = portraitFrames[t.portraitIndex];

                    dialogText.text = line.Substring(0, j + 1);
                    yield return new WaitForSeconds(0.025f);
                }
                dialogPortrait.sprite = portraitFrames[t.portraitIndex];
                dialogText.text = line;

                yield return WaitForKeyPress();
            }
        }
    }

    IEnumerator WaitForKeyPress()
    {
        while (!Input.GetKeyDown(KeyCode.E))
            yield return null;
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
