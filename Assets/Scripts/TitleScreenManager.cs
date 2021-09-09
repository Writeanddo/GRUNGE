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
    int activeMenuScreen = 0;
    public AudioClip titleIntro;
    public AudioMixer mixer;
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

    int sfxEnabled;
    int musicEnabled;

    int camTargetPosition;


    List<IEnumerator> activeButtonCoroutines;

    Toggle musicToggle;
    Toggle sfxToggle;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        music = GameObject.Find("Music").GetComponent<AudioSource>();
        cam = FindObjectOfType<Camera>().transform;
        music.PlayOneShot(titleIntro);
        sfx = GameObject.Find("SFX").GetComponent<AudioSource>();
        blackout = GameObject.Find("ScreenBlackout").GetComponent<Image>();
        musicToggle = GameObject.Find("MusicToggle").GetComponent<Toggle>();
        sfxToggle = GameObject.Find("SFXToggle").GetComponent<Toggle>();

        activeButtonCoroutines = new List<IEnumerator>();

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
        mixer.SetFloat("PrioritySFXVolume", volume);
    }


    private void FixedUpdate()
    {
        if (activeMenuScreen == 0)
        {
            cam.transform.position = Vector2.Lerp(cam.transform.position, new Vector3(0, 0, -10), 0.3f);
            camArrived = (Vector2.Distance(cam.transform.position, new Vector2(0, 0)) < 1);
        }
        else
        {
            cam.transform.position = Vector2.Lerp(cam.transform.position, new Vector3(75, 0, -10), 0.3f);
            camArrived = (Vector2.Distance(cam.transform.position, new Vector2(75, 0)) < 1);
        }
    }

    public IEnumerator MenuScreenButtonTransition(int nextScreen, int buttonToRemainOnNewScreen, MenuScreen from, MenuScreen to)
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

    }

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
