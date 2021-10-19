using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public int furthestUnlockedLevel;
    public int selectedLevelIndex;
    public string[] levelPreviewVideos;
    
    Image[] dotBorders = new Image[6];
    Image[] dotConnecters = new Image[5];
    Image dot;
    Text vhsText;

    Animator tvStaticAnimation;
    Animator vhsIconAnimation;
    VideoPlayer videoPlayer;

    string storedVhsText = "";
    int vhsTextOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        tvStaticAnimation = GameObject.Find("LevelPreviewStatic").GetComponent<Animator>();
        videoPlayer = GameObject.Find("LevelPreviewVideoPlayer").GetComponent<VideoPlayer>();
        vhsIconAnimation = videoPlayer.GetComponent<Animator>();
        vhsText = GameObject.Find("VHSStatsText").GetComponent<Text>();

        for(int i = 1; i <= 6; i++)
            dotBorders[i-1] = transform.GetChild(i).GetComponent<Image>();
        for(int i = 7; i <= 11; i++)
            dotConnecters[i-7] = transform.GetChild(i).GetComponent<Image>();

        dot = GameObject.Find("SelectedLevelDot").GetComponent<Image>();

        if (!PlayerPrefs.HasKey("GRUNGE_FURTHEST_UNLOCKED_LEVEL"))
            PlayerPrefs.SetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL", 0);
        else
            PlayerPrefs.SetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL", 0);

        StartCoroutine(VHSTextLoop());
        UpdateUnlockedLevels(PlayerPrefs.GetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL"));
    }

    public void UpdateUnlockedLevels(int furthestUnlockedLevel)
    {
        this.furthestUnlockedLevel = furthestUnlockedLevel;

        for (int i = 0; i <= furthestUnlockedLevel; i++)
        {
            dotBorders[i].color = Color.white;
            if(i > 0)
                dotConnecters[i - 1].color = Color.white;
        }

        SetSelectedLevel(furthestUnlockedLevel);
    }

    public void SetSelectedLevel(int index)
    {
        if (index > furthestUnlockedLevel)
            return;

        selectedLevelIndex = index;
        dot.rectTransform.anchoredPosition = new Vector2((index * 128f) - 320f, -80f);

        tvStaticAnimation.Play("LevelSelectStatic", -1, 0);

        vhsTextOffset = 0;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, levelPreviewVideos[index]+".mp4");
        RestartPreview();
    }

    public void LoadSelectedLevel()
    {
        StartCoroutine(WaitAndLoadLevel());
    }

    IEnumerator WaitAndLoadLevel()
    {
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene(selectedLevelIndex + 2);
    }

    public void RestartPreview()
    {
        vhsIconAnimation.Play("LevelPreviewAnimation", -1, 0);
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    IEnumerator VHSTextLoop()
    {
        storedVhsText = "     TIME: 1:00     KILLS: 21";
        //storedVhsText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        vhsTextOffset = 0;

        while(true)
        {
            string s = "";
            s += storedVhsText.Substring(vhsTextOffset, storedVhsText.Length - vhsTextOffset);
            s += storedVhsText.Substring(0, vhsTextOffset);
            s = s.Substring(0, Mathf.Min(20, s.Length));

            vhsText.text = s;
            yield return new WaitForSeconds(0.1f);

            vhsTextOffset++;
            if (vhsTextOffset >= storedVhsText.Length)
                vhsTextOffset = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
