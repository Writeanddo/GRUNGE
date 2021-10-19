using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    GameManager gm;
    PlayerController ply;

    // Start is called before the first frame update
    void Start()
    {
        ply = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator IntroDialog()
    {
        yield return gm.WaitForTextCompletion("TutorialIntro");
        ply.canMove = true;
    }
}
