using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using io.newgrounds;

public class LevelSelectManager : MonoBehaviour
{
    public int furthestUnlockedLevel;
    public int selectedLevelIndex = 0;
    public string[] levelPreviewVideos;
    public Sprite[] unlockedIcons;
    public Sprite[] selectedIcons;

    core ngCore;

    Image[] dotIcons = new Image[6];
    Text vhsText;
    Text endlessModeText;

    Animator tvStaticAnimation;
    Animator vhsIconAnimation;
    VideoPlayer videoPlayer;
    TitleScreenManager tsm;

    string storedVhsText = "";
    string[] levelNames = new string[6] { "TUTORIAL", "THE FRONT YARD", "THE HOUSE", "THE BASEMENT", "THE PIT", "ENDLESS" };
    public string[] levelTimeRecords = new string[3];
    public string[] levelKillRecords = new string[3];
    int vhsTextOffset;
    bool endlessModeActive;

    IEnumerator loadVideo;

    // Start is called before the first frame update
    void Start()
    {
        ngCore = GetComponent<core>();
        tsm = FindObjectOfType<TitleScreenManager>();
        tsm.LoadSaveData();
        tvStaticAnimation = GameObject.Find("LevelPreviewStatic").GetComponent<Animator>();
        videoPlayer = GameObject.Find("LevelPreviewVideoPlayer").GetComponent<VideoPlayer>();
        vhsIconAnimation = videoPlayer.GetComponent<Animator>();
        vhsText = GameObject.Find("VHSStatsText").GetComponent<Text>();
        endlessModeText = GameObject.Find("EndlessModeText").GetComponent<Text>();

        for (int i = 1; i <= 6; i++)
            dotIcons[i - 1] = transform.GetChild(i).GetComponent<Image>();

        // Get high score data if it exists
        for(int i = 0; i < 3; i++)
        {
            // Get time
            string timeText;
            if (tsm.saveVars.hiScoreTimes[i] == 0)
                timeText = "N/A";
            else
            {
                float bestTime = tsm.saveVars.hiScoreTimes[i];
                int minutes = Mathf.FloorToInt(bestTime / 60F);
                int seconds = Mathf.FloorToInt(bestTime - minutes * 60);
                int milliseconds = Mathf.FloorToInt(((bestTime - (minutes * 60) - seconds)) * 100);
                string niceTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
                timeText = niceTime;
            }

            levelTimeRecords[i] = timeText;

            // Get kills
            levelKillRecords[i] = tsm.saveVars.hiScoreKills[i].ToString();
        }


        if (PlayerPrefs.HasKey("GRUNGE_IS_ENDLESS") && PlayerPrefs.GetInt("GRUNGE_IS_ENDLESS") == 1)
            endlessModeActive = true;

        UpdateUnlockedLevels(tsm.saveVars.furthestUnlockedLevel);
        StartCoroutine(VHSTextLoop());
    }

    
    
    void GetScoreboardValues()
    {



        // Currently unused - hi score is currently stored in JSON file but will fix this properly when I have time
        /*
        for (int i = 0; i < 3; i++)
        {
            io.newgrounds.components.ScoreBoard.getScores killsScore = new io.newgrounds.components.ScoreBoard.getScores();
            io.newgrounds.components.ScoreBoard.getScores timeScore = new io.newgrounds.components.ScoreBoard.getScores();
            switch (i)
            {
                // Front lawn
                case (0):
                    killsScore.id = 10983;
                    timeScore.id = 10982;
                    break;
                case (1):
                    killsScore.id = 10984;
                    timeScore.id = 10985;
                    break;
                case (2):
                    killsScore.id = 10986;
                    timeScore.id = 10987;
                    break;
            }

            //killsScore.callWith(ngCore, AssignScoreboardValues);
            //timeScore.callWith(ngCore, AssignScoreboardValues);
            
        }*/
    }

    void AssignScoreboardValues(io.newgrounds.components.ScoreBoard.getScores result)
    {
        
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
        tsm.saveVars.furthestUnlockedLevel = furthestUnlockedLevel;

        for (int i = 0; i <= furthestUnlockedLevel; i++)
        {
            if (i == selectedLevelIndex)
                dotIcons[i].sprite = selectedIcons[i];
            else
                dotIcons[i].sprite = unlockedIcons[i];
        }

        // Change selected level to be the last level we were in
        int index = selectedLevelIndex;
        if (PlayerPrefs.HasKey("GRUNGE_LAST_SELECTED_LEVEL"))
        {
            index = PlayerPrefs.GetInt("GRUNGE_LAST_SELECTED_LEVEL");
        }

        SetSelectedLevel(index);
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
            string timeText = "N/A";
            string killsText = "N/A";
            if(selectedLevelIndex > 0 && selectedLevelIndex < 4)
            {
                timeText = levelTimeRecords[selectedLevelIndex - 1];
                killsText = levelKillRecords[selectedLevelIndex - 1];
            }

            storedVhsText = "     " + levelNames[selectedLevelIndex] + "     TIME: "+timeText+"     KILLS: "+killsText;
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
        PlayerPrefs.SetInt("GRUNGE_LAST_SELECTED_LEVEL", selectedLevelIndex);
        PlayerPrefs.SetInt("GRUNGE_LOAD_TO_LEVEL_SELECT", 1);
        if (endlessModeActive)
            PlayerPrefs.SetInt("GRUNGE_IS_ENDLESS", 1);
        else
            PlayerPrefs.SetInt("GRUNGE_IS_ENDLESS", 0);

        PlayerPrefs.Save();
        tsm.WriteSaveData();
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
        while (loadVideo == null)
            yield return null;

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
