using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.IO;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    public GameManager.SaveDataVariables saveVars;
    public RectTransform quitButton;
    public AudioClip titleIntro;
    public AudioMixer mixer;
    public AudioClip[] uiSfx;
    public Animator screenTransitionFX;
    public RectTransform levelSelectScreen;
    public RectTransform topScreen;
    public RectTransform thumbnailScreen;
    public RectTransform creditsScreen;
    public RectTransform optionsScreen;
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI fullscreenToggleText;

    Animator thumbnailAnimator;
    AudioSource music;
    AudioSource sfx;
    Image blackout;
    Transform cam;
    // 0: top menu
    // 1: level select
    // 2: options
    // 3: credits
    // 

    // Start is called before the first frame update

    bool kPressed;
    bool lerpKG;
    bool loadingGame;
    public bool camArrived;
    public bool performingScreenTransition;

    int camTargetPosition;


    List<IEnumerator> activeButtonCoroutines;

    Slider musicSlider;
    Slider sfxSlider;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        music = GameObject.Find("Music").GetComponent<AudioSource>();
        cam = FindObjectOfType<Camera>().transform;
        //music.PlayOneShot(titleIntro);
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
        blackout = GameObject.Find("ScreenBlackout").GetComponent<Image>();
        musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();
        sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
        thumbnailAnimator = thumbnailScreen.GetComponent<Animator>();
        activeButtonCoroutines = new List<IEnumerator>();

        // First time setup
        if (!PlayerPrefs.HasKey("GRUNGE_SFX_VOLUME"))
        {
            PlayerPrefs.SetInt("GRUNGE_SFX_VOLUME", -8);
            PlayerPrefs.SetInt("GRUNGE_MUSIC_VOLUME", -12);
        }

        // Check if loading from title screen or if loading from level
        int loadPosition = PlayerPrefs.GetInt("GRUNGE_LOAD_TO_LEVEL_SELECT");

        if (loadPosition == 0)
            StartCoroutine(ThumbnailAnimationCoroutine());
        else
        {
            if (loadPosition == 1)
                levelSelectScreen.anchoredPosition = Vector2.zero;
            else if (loadPosition == 2)
                creditsScreen.anchoredPosition = Vector2.zero;
            music.Play();
            StartCoroutine(FadeInScreen());
        }

        // Update volume slider levels
        sfxSlider.value = PlayerPrefs.GetInt("GRUNGE_SFX_VOLUME") / 4;
        musicSlider.value = PlayerPrefs.GetInt("GRUNGE_MUSIC_VOLUME") / 4;
        UpdateMusicVolume();
        UpdateSFXVolume();

        versionText.text = "Version " + Application.version;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Destroy(quitButton.gameObject);
            Destroy(fullscreenToggleText.gameObject);
        }
    }

    public void PlaySfxTestSound()
    {
        if (optionsScreen.anchoredPosition == Vector2.zero)
        {
            sfx.Stop();
            sfx.PlayOneShot(uiSfx[5]);
        }
    }

    IEnumerator ThumbnailAnimationCoroutine()
    {
        sfx.PlayOneShot(uiSfx[4]);
        thumbnailScreen.anchoredPosition = Vector2.zero;
        yield return FadeInScreen();
        thumbnailAnimator.Play("ThumbnailScreenSweep", -1, 0);
        yield return new WaitForSeconds(4);
        StartCoroutine(MoveToScreen(thumbnailScreen.GetComponent<MenuScreen>(), topScreen.GetComponent<MenuScreen>()));
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F10))
                SetFullscreenMode(!Screen.fullScreen);
        }
    }

    void SetFullscreenMode(bool isFullscreen)
    {
        int width = 1024;
        int height = 720;
        if (isFullscreen)
        {
            width = Screen.currentResolution.width;
            height = Screen.currentResolution.height;
        }

        Screen.SetResolution(width, height, isFullscreen);
    }

    public void PlayMusic()
    {
        music.Play();
    }

    public void UpdateMusicVolume()
    {
        if (musicSlider == null)
            return;

        int volume = (int)musicSlider.value * 4;

        if (volume == -40)
            volume = -80;

        PlayerPrefs.SetInt("GRUNGE_MUSIC_VOLUME", volume);
        mixer.SetFloat("MusicVolume", volume);
    }
    public void UpdateSFXVolume()
    {
        if (sfxSlider == null)
            return;

        int volume = (int)sfxSlider.value * 4;

        if (volume == -40)
            volume = -80;

        PlayerPrefs.SetInt("GRUNGE_SFX_VOLUME", volume);
        mixer.SetFloat("SFXVolume", volume);
        mixer.SetFloat("PrioritySFXVolume", volume);
    }




    /*public IEnumerator MenuScreenButtonTransition(int nextScreen, int buttonToRemainOnNewScreen, MenuScreen from, MenuScreen to)
    {
        RectTransform[] fromRects = new RectTransform[from.buttons.Length];
        RectTransform[] toRects = new RectTransform[to.buttons.Length];

        // Hide all irrelevant menu screens
        for(int i = 0; i < menuScreens.Length; i++)
        {
            if (menuScreens[i].gameObject == from.gameObject || menuScreens[i].gameObject == to.gameObject)
            {
                menuScreens[i].anchoredPosition = new Vector2(menuScreens[i].anchoredPosition.x, 0);
                    continue;
            }

            menuScreens[i].anchoredPosition = new Vector2(menuScreens[i].anchoredPosition.x, 1000);
        }

        // Disable all interactions from buttons
        for(int i = 0; i < to.buttons.Length; i++)
        {
            to.buttons[i].interactable = false;
            toRects[i] = to.buttons[i].GetComponent<RectTransform>();
        }
        for (int i = 0; i < from.buttons.Length; i++)
        {
            from.buttons[i].interactable = false;
            fromRects[i] = from.buttons[i].GetComponent<RectTransform>();
        }

        // Hide all buttons on 'to' screen except for specified one
        for (int i = 0; i < to.buttons.Length; i++)
            if (i != buttonToRemainOnNewScreen)
                toRects[i].anchoredPosition = new Vector2(0, -10000);

        // Prepare button lerp coroutines
        activeButtonCoroutines = new List<IEnumerator>();
        for (int i = 0; i < from.buttons.Length; i++)
        {
            // Skip the button we just clicked
            if (i == from.activeButtonIndex)
                continue;
            activeButtonCoroutines.Add(LerpButtonTowards(fromRects[i], false, fromRects[i].anchoredPosition, fromRects[i].anchoredPosition + new Vector2(from.moveAmount, 0)));
        }

        // Start all button lerp coroutines except for one so we can track when they're finished
        for (int i = 0; i < activeButtonCoroutines.Count - 1; i++)
            StartCoroutine(activeButtonCoroutines[i]);

        if (from.buttons.Length > 1)
            yield return StartCoroutine(activeButtonCoroutines[activeButtonCoroutines.Count - 1]);
        //yield return new WaitForSeconds(0.5f);

        // Set the active screen so we can start moving the camera now
        activeMenuScreen = nextScreen;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while (!camArrived)
            yield return null;

        // Lerp new screen buttons into view
        activeButtonCoroutines.Clear();
        for (int i = 0; i < to.buttons.Length; i++)
        {
            // Skip the button we just clicked
            if (i == buttonToRemainOnNewScreen)
                continue;
            activeButtonCoroutines.Add(LerpButtonTowards(toRects[i], true, to.buttonsActivePosition[i] + new Vector2(to.moveAmount, 0), to.buttonsActivePosition[i]));
        }

        // Again, start all button lerp coroutines except for one so we can track when they're finished
        for (int i = 0; i < activeButtonCoroutines.Count - 1; i++)
            StartCoroutine(activeButtonCoroutines[i]);

        if(to.buttons.Length > 1)
            yield return StartCoroutine(activeButtonCoroutines[activeButtonCoroutines.Count - 1]);

        // Enable all interactions from buttons
        foreach (Button b in to.buttons)
            b.interactable = true;
        foreach (Button b in from.buttons)
            b.interactable = true;

    }*/

    IEnumerator LerpButtonTowards(RectTransform button, bool active, Vector3 startPosition, Vector3 endPosition)
    {
        startPosition = new Vector3(startPosition.x, startPosition.y, 0);
        endPosition = new Vector3(endPosition.x, endPosition.y, 0);
        button.anchoredPosition = startPosition;
        while (Vector3.Distance(button.anchoredPosition, endPosition) >= 0.1f)
        {
            button.anchoredPosition = Vector3.Lerp(button.anchoredPosition, endPosition, 0.4f);
            yield return new WaitForFixedUpdate();
        }
        print("Finished moving button");
        button.anchoredPosition = endPosition;

        if (!active)
            button.anchoredPosition = new Vector2(0, -10000);
    }

    public void ShowTopMenu()
    {
        //SetMenuScreen(0);
    }

    public void StopMusic()
    {
        music.Stop();
    }

    public void OpenArtistURL(string url)
    {
        //Application.OpenURL(url);
        //Application.ExternalEval("window.open("+url+");");
    }

    public IEnumerator MoveToScreen(MenuScreen from, MenuScreen to)
    {
        sfx.PlayOneShot(uiSfx[3]);
        performingScreenTransition = true;
        screenTransitionFX.Play("ScreenTransition", -1, 0);
        yield return new WaitForSeconds(0.25f);
        from.rectTransform.anchoredPosition = new Vector2(0, 1000);
        to.rectTransform.anchoredPosition = Vector2.zero;
        yield return new WaitForSeconds(0.25f);
        performingScreenTransition = false;
    }

    IEnumerator FadeInScreen()
    {
        while (blackout.color.a > 0)
        {
            blackout.color = new Color(0, 0, 0, blackout.color.a - 0.075f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
        }
        blackout.rectTransform.anchoredPosition = new Vector2(0, -3000);

    }

    // POTENTIALLY JANKY WORKAROUND IN USE HERE https://itch.io/t/140214/persistent-data-in-updatable-webgl-games
    public void LoadSaveData()
    {
        string prefix = @"idbfs/GRUNGE";
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            prefix = Application.persistentDataPath;

        if (!File.Exists(prefix + @"/grungedata.json"))
        {
            saveVars = new GameManager.SaveDataVariables();
            return;
        }
        string json = File.ReadAllText(prefix + @"/grungedata.json");
        saveVars = JsonUtility.FromJson<GameManager.SaveDataVariables>(json);
    }

    public void WriteSaveData()
    {
        string prefix = @"idbfs/GRUNGE";
        //Debug.LogError(prefix + ", " + Application.persistentDataPath);
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            prefix = Application.persistentDataPath;
        else if (!Directory.Exists(prefix))
            Directory.CreateDirectory(prefix);

        print(prefix + @"/grungedata.json");
        string json = JsonUtility.ToJson(saveVars);
        File.WriteAllText(prefix + @"/grungedata.json", json);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
