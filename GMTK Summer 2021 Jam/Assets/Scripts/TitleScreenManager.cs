using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class TitleScreenManager : MonoBehaviour
{
    public RectTransform[] menuScreens;
    public RectTransform kgScreen;
    public RectTransform quitButton;
    int activeMenuScreen = 4;
    public AudioClip titleIntro;
    public AudioMixer mixer;
    AudioSource music;
    AudioSource sfx;
    Image blackout;
    // 0: top menu
    // 1: level select
    // 2: options
    // 3: credits
    // 

    // Start is called before the first frame update

    bool kPressed;
    bool lerpKG;
    bool loadingGame;

    int sfxEnabled;
    int musicEnabled;

    Toggle musicToggle;
    Toggle sfxToggle;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        music = GameObject.Find("Music").GetComponent<AudioSource>();
        music.PlayOneShot(titleIntro);
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
        blackout = GameObject.Find("ScreenBlackout").GetComponent<Image>();
        musicToggle = GameObject.Find("MusicToggle").GetComponent<Toggle>();
        sfxToggle = GameObject.Find("SFXToggle").GetComponent<Toggle>();

        // First time setup
        if (!PlayerPrefs.HasKey("GRUNGE_SFX_ENABLED"))
        {
            PlayerPrefs.SetInt("GRUNGE_SFX_ENABLED", 1);
            PlayerPrefs.SetInt("GRUNGE_MUSIC_ENABLED", 1);
        }

        sfxEnabled = PlayerPrefs.GetInt("GRUNGE_SFX_ENABLED");
        musicEnabled = PlayerPrefs.GetInt("GRUNGE_MUSIC_ENABLED");

        if (sfxEnabled == 0)
            sfxToggle.isOn = false;    
        if (musicEnabled == 0)
            musicToggle.isOn = false;

        ToggleMusicEnabled();
        ToggleSFXEnabled();
    }

    public void PlayMusic()
    {
        music.Play();
    }

    public void ToggleMusicEnabled()
    {
        float volume;
        int state = 0;
        if (musicToggle.isOn)
        {
            state = 1;
            volume = -5.0f;
        }
        else
            volume = -80.0f;

        PlayerPrefs.SetInt("GRUNGE_MUSIC_ENABLED", state);
        mixer.SetFloat("MusicVolume", volume);
    }
    public void ToggleSFXEnabled()
    {
        int state = 0;
        float volume;
        if (sfxToggle.isOn)
        {
            state = 1;
            volume = 0.0f;
        }
        else
            volume = -80.0f;

        PlayerPrefs.SetInt("GRUNGE_SFX_ENABLED", state);
        mixer.SetFloat("SFXVolume", volume);
    }


    private void Update()
    {
        /*if (activeMenuScreen == 3)
        {
            if (!lerpKG)
            {
                if (Input.GetKeyDown(KeyCode.K))
                    kPressed = true;
                else if (kPressed && Input.GetKeyDown(KeyCode.G))
                    lerpKG = true;
                else if (Input.anyKeyDown)
                    kPressed = false;
            }
            else
                kgScreen.anchoredPosition = Vector2.Lerp(kgScreen.anchoredPosition, new Vector2(-276f, 165f), 0.1f);
        }
        else
        {
            lerpKG = false;
            kPressed = false;
        }*/
    }

    public void SetMenuScreen(int level)
    {
        if (loadingGame)
            return;

        activeMenuScreen = level;
        for (int i = 0; i < menuScreens.Length; i++)
        {
            if (i == activeMenuScreen)
            {
                menuScreens[i].anchoredPosition = Vector2.zero;
            }
            else if (i != 0)
                menuScreens[i].anchoredPosition = new Vector2(0, -1500);
        }
    }

    public void ShowTopMenu()
    {
        SetMenuScreen(0);
    }

    public void StartGame()
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        loadingGame = true;
        blackout.rectTransform.anchoredPosition = Vector2.zero;
        while (blackout.color.a < 1)
        {
            blackout.color = new Color(0, 0, 0, blackout.color.a + 0.075f);
            music.volume -= 0.075f;
            yield return new WaitForSeconds(0.05f);
        }
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
