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
    public Sprite[] unlockedIcons;
    public Sprite[] selectedIcons;

    Image[] dotIcons = new Image[6];
    Text vhsText;

    Animator tvStaticAnimation;
    Animator vhsIconAnimation;
    VideoPlayer videoPlayer;

    string storedVhsText = "";
    string[] levelNames = new string[6] { "TUTORIAL", "THE FRONT YARD", "THE HOUSE", "THE BASEMENT", "THE PIT", "ENDLESS" };
    int vhsTextOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        tvStaticAnimation = GameObject.Find("LevelPreviewStatic").GetComponent<Animator>();
        videoPlayer = GameObject.Find("LevelPreviewVideoPlayer").GetComponent<VideoPlayer>();
        vhsIconAnimation = videoPlayer.GetComponent<Animator>();
        vhsText = GameObject.Find("VHSStatsText").GetComponent<Text>();

        for(int i = 1; i <= 6; i++)
            dotIcons[i-1] = transform.GetChild(i).GetComponent<Image>();

        if (!PlayerPrefs.HasKey("GRUNGE_FURTHEST_UNLOCKED_LEVEL"))
            PlayerPrefs.SetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL", 0);

        StartCoroutine(VHSTextLoop());
        UpdateUnlockedLevels(PlayerPrefs.GetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL"));
    }

    public void UpdateUnlockedLevels(int furthestUnlockedLevel)
    {
        this.furthestUnlockedLevel = furthestUnlockedLevel;

        for (int i = 0; i <= furthestUnlockedLevel; i++)
        {
            if (i == selectedLevelIndex)
                dotIcons[i].sprite = selectedIcons[i];
            else
                dotIcons[i].sprite = unlockedIcons[i];
        }

        SetSelectedLevel(furthestUnlockedLevel);
    }

    public void SetSelectedLevel(int index)
    {
        if (index > furthestUnlockedLevel)
            return;

        selectedLevelIndex = index;

        for (int i = 0; i <= furthestUnlockedLevel; i++)
        {
            if (i == selectedLevelIndex)
                dotIcons[i].sprite = selectedIcons[i];
            else
                dotIcons[i].sprite = unlockedIcons[i];
        }

        tvStaticAnimation.Play("LevelSelectStatic", -1, 0);

        vhsTextOffset = 0;
        storedVhsText = "     "+levelNames[selectedLevelIndex]+"     TIME: N/A     KILLS: N/A";
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, levelPreviewVideos[index]+".mp4");
        RestartPreview();
    }

    public void LoadSelectedLevel()
    {
        StartCoroutine(WaitAndLoadLevel());
    }

    IEnumerator WaitAndLoadLevel()
    {
        PlayerPrefs.SetInt("GRUNGE_LOAD_TO_LEVEL_SELECT", 1);
        PlayerPrefs.Save();
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
        storedVhsText = "     TUTORIAL     TIME: N/A     KILLS: N/A";
        //storedVhsText = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        vhsTextOffset = 0;

        while(true)
        {
            string s = "";
            s += storedVhsText.Substring(vhsTextOffset, storedVhsText.Length - vhsTextOffset);
            s += storedVhsText.Substring(0, vhsTextOffset);
            s = s.Substring(0, Mathf.Min(22, s.Length));

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
