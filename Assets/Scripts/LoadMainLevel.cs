using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadMainLevel : MonoBehaviour
{
    Text focusText;
    VideoPlayer vp;

    // Start is called before the first frame update
    void Start()
    {
        focusText = FindObjectOfType<Text>();
        vp = FindObjectOfType<VideoPlayer>();
        //focusText.color = Color.clear;
        StartCoroutine(WaitAndLoad());
    }
    IEnumerator WaitAndLoad()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            while (!Input.GetMouseButtonDown(0))
            {
                focusText.color = Color.white;
                yield return null;
            }
        }

        // So we know we're loading from the title screen
        PlayerPrefs.SetInt("GRUNGE_LOAD_TO_LEVEL_SELECT", 0);

        focusText.color = Color.clear;
        vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, "TubLogo.mp4");
        vp.Play();
        yield return new WaitForSeconds(4.25f);
        SceneManager.LoadScene(1);
    }
}
