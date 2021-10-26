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
    Text endlessModeText;

    Animator tvStaticAnimation;
    Animator vhsIconAnimation;
    VideoPlayer videoPlayer;

    string storedVhsText = "";
    string[] levelNames = new string[6] { "TUTORIAL", "THE FRONT YARD", "THE HOUSE", "THE BASEMENT", "THE PIT", "ENDLESS" };
    int vhsTextOffset;
    bool endlessModeActive;

    IEnumerator loadVideo;

    // Start is called before the first frame update
    void Start()
    {
        tvStaticAnimation = GameObject.Find("LevelPreviewStatic").GetComponent<Animator>();
        videoPlayer = GameObject.Find("LevelPreviewVideoPlayer").GetComponent<VideoPlayer>();
        vhsIconAnimation = videoPlayer.GetComponent<Animator>();
        vhsText = GameObject.Find("VHSStatsText").GetComponent<Text>();
        endlessModeText = GameObject.Find("EndlessModeText").GetComponent<Text>();

        for (int i = 1; i <= 6; i++)
            dotIcons[i - 1] = transform.GetChild(i).GetComponent<Image>();

        if (!PlayerPrefs.HasKey("GRUNGE_FURTHEST_UNLOCKED_LEVEL"))
            PlayerPrefs.SetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL", 0);

        if (PlayerPrefs.HasKey("GRUNGE_IS_ENDLESS") && PlayerPrefs.GetInt("GRUNGE_IS_ENDLESS") == 1)
            endlessModeActive = true;

        StartCoroutine(VHSTextLoop());
        UpdateUnlockedLevels(PlayerPrefs.GetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL"));

        // TEMP TEMP TEMP TEMP TEMP
        UpdateUnlockedLevels(5);
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateUnlockedLevels(5);
        }
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

        SetSelectedLevel(Mathf.Min(furthestUnlockedLevel, 4));
    }

    public void SetSelectedLevel(int index)
    {
        if (index > furthestUnlockedLevel)
            return;

        selectedLevelIndex = index;

        for (int i = 0; i <= Mathf.Min(furthestUnlockedLevel, 4); i++)
        {
            if (i == selectedLevelIndex)
                dotIcons[i].sprite = selectedIcons[i];
            else if (i != 5 || (i == 5 && endlessModeActive))
                dotIcons[i].sprite = unlockedIcons[i];
        }

        if (furthestUnlockedLevel == 5)
        {
            if (!endlessModeActive)
                dotIcons[5].sprite = unlockedIcons[5];
            else
                dotIcons[5].sprite = selectedIcons[5];
        }

        if (endlessModeActive)
        {
            storedVhsText = "     " + levelNames[selectedLevelIndex] + "     TIME: N/A     KILLS: N/A";
            levelPreviewVideos[0] = "EndlessNA";
            levelPreviewVideos[4] = "EndlessNA";
        }
        else
        {
            storedVhsText = "          " + levelNames[selectedLevelIndex] + "          ";
            levelPreviewVideos[0] = "Level0Preview";
            levelPreviewVideos[4] = "Level4Preview";
        }

        if (loadVideo != null)
            StopCoroutine(loadVideo);
        loadVideo = WaitForVideoClipLoad(index);
        StartCoroutine(loadVideo);
    }

    IEnumerator WaitForVideoClipLoad(int index)
    {
        tvStaticAnimation.Play("LevelSelectStatic", -1, 0);
        videoPlayer.Stop();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, levelPreviewVideos[index] + ".mp4");
        videoPlayer.Play();
        vhsTextOffset = 0;
        yield return new WaitForSeconds(0.1f);
        while (!videoPlayer.isPrepared)
            yield return null;
        RestartPreview();
        yield return new WaitForSeconds(0.05f);
        tvStaticAnimation.Play("LevelSelectStaticBlank", -1, 0);
    }

    public void LoadSelectedLevel()
    {
        if (endlessModeActive && (selectedLevelIndex == 0 || selectedLevelIndex == 4))
            return;

        GetComponent<MenuScreen>().ButtonWasPressed(0);
        FindObjectOfType<TitleScreenManager>().StopMusic();
        StartCoroutine(WaitAndLoadLevel());
    }

    IEnumerator WaitAndLoadLevel()
    {
        PlayerPrefs.SetInt("GRUNGE_LOAD_TO_LEVEL_SELECT", 1);
        if (endlessModeActive)
            PlayerPrefs.SetInt("GRUNGE_IS_ENDLESS", 1);
        else
            PlayerPrefs.SetInt("GRUNGE_IS_ENDLESS", 0);

        PlayerPrefs.Save();
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene(selectedLevelIndex + 2);
    }

    public void RestartPreview()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
        vhsIconAnimation.Play("LevelPreviewAnimation", -1, 0);
    }

    IEnumerator VHSTextLoop()
    {
        vhsTextOffset = 0;

        while (true)
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

    public void ToggleEndlessMode()
    {
        if (furthestUnlockedLevel < 5)
            return;

        endlessModeActive = !endlessModeActive;


        string t = "";
        if (endlessModeActive)
            t = "ENDLESS";

        endlessModeText.text = t;

        SetSelectedLevel(selectedLevelIndex);
    }
}
