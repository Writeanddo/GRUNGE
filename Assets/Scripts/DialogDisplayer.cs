using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogDisplayer : MonoBehaviour
{
    public TextAsset sourceFile;
    TextboxManager text;

    // Start is called before the first frame update
    void Start()
    {
        text = FindObjectOfType<TextboxManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            TextboxManager.TextData[] t = JsonHelper.FromJson<TextboxManager.TextData>(sourceFile.text);
            text.StartCoroutine(text.PrintAllText(t));
        }
    }
}
