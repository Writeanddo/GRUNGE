using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Transform cam;
    PlayerController ply;
    Slider gooSlider;
    Image gooFill;
    Slider healthSlider;
    Image healthFill;
    Image shieldImage;

    Image screenBlackout;
    Image quitBlackout;
    Image gameOverImage;
    Image tryAgainImage;
    Image retryImage;

    RectTransform quitYes;
    RectTransform quitNo;
    EnemyScript heldEnemy;
    AudioSource musicSource;
    AudioSource sfxSource;
    Text pauseText;

    public bool paused;
    public bool gameOver;
    public Sprite[] shieldUiImages;
    public AudioClip music;

    int storedGooAmount;
    int storedHealthAmount;
    Color gooColor;
    Color healthColor;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        cam = transform.GetChild(0).GetChild(0);
        ply = FindObjectOfType<PlayerController>();
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        healthFill = healthSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        gooSlider = GameObject.Find("GooSlider").GetComponent<Slider>();
        gooFill = gooSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        gooColor = gooFill.color;
        healthColor = healthFill.color;
        sfxSource = GameObject.Find("GameSFX").GetComponent<AudioSource>();
        musicSource = GameObject.Find("GameMusic").GetComponent<AudioSource>();

        screenBlackout = GameObject.Find("ScreenBlackout").GetComponent<Image>();
        quitBlackout = GameObject.Find("QuitPanel").GetComponent<Image>();
        pauseText = GameObject.Find("QuitText").GetComponent<Text>();
        quitYes = GameObject.Find("QuitYesButton").GetComponent<RectTransform>();
        quitNo = GameObject.Find("QuitNoButton").GetComponent<RectTransform>();
        shieldImage = GameObject.Find("ShieldImage").GetComponent<Image>();
        gameOverImage = GameObject.Find("GameOverImage").GetComponent<Image>();
        tryAgainImage = GameObject.Find("TryAgainImage").GetComponent<Image>();

        StartCoroutine(LevelStartSequence());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Health update
        healthSlider.value = Mathf.Lerp(healthSlider.value, (float)ply.stats.health / ply.stats.maxHealth, 0.1f);
        healthFill.color = Color.Lerp(healthFill.color, healthColor, 0.1f);
        if (ply.stats.health > storedHealthAmount)
            healthFill.color = new Color(1, 0, 1);
        else if (ply.stats.health < storedHealthAmount)
            healthFill.color = new Color(0.5f, 0, 0);
        storedHealthAmount = ply.stats.health;

        if (!gameOver)
        {
            // Goo update
            gooSlider.value = Mathf.Lerp(gooSlider.value, (float)ply.stats.goo / ply.stats.maxGoo, 0.1f);
            gooFill.color = Color.Lerp(gooFill.color, gooColor, 0.1f);
            if (ply.stats.goo > storedGooAmount)
                gooFill.color = new Color(0, 0.75f, 0);
            else if (ply.stats.goo < storedGooAmount)
                gooFill.color = new Color(0.75f, 0.75f, 0);
            storedGooAmount = ply.stats.goo;

            // Shield update
            shieldImage.rectTransform.localScale = Vector2.Lerp(shieldImage.rectTransform.localScale, Vector2.one, 0.25f);
            if (ply.heldObject != null && ply.heldObject.tag == "Enemy")
            {
                if (heldEnemy == null)
                {
                    heldEnemy = ply.heldObject.GetComponent<EnemyScript>();
                    shieldImage.rectTransform.localScale = new Vector2(1.2f, 1.2f);
                }

                shieldImage.sprite = shieldUiImages[heldEnemy.stats.currentShieldValue];
            }
            else if (heldEnemy != null)
            {
                heldEnemy = null;
                shieldImage.sprite = shieldUiImages[0];
            }

            // Pause game
            if (paused && pauseText.text == "")
            {
                Time.timeScale = 0;
                pauseText.text = "PAUSED";
                quitBlackout.color = new Color(0, 0, 0, 0.5f);
            }
            else if (!paused && pauseText.text != "")
            {
                pauseText.text = "";
                quitBlackout.color = new Color(0, 0, 0, 0);
            }
        }
    }

    public void ShieldHit()
    {
        shieldImage.rectTransform.localScale = new Vector2(1.1f, 1.1f);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            paused = !paused;
            Cursor.visible = !paused;
            if (!paused)
                Time.timeScale = 1;
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        musicSource.clip = music;
        musicSource.Play();
    }


    public void ScreenShake(float intensity)
    {
        StartCoroutine(ScreenShakeCoroutine(intensity));
    }

    IEnumerator ScreenShakeCoroutine(float intensity)
    {
        for (int i = 0; i < 10; i++)
        {
            cam.localPosition = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) * intensity;
            intensity /= 1.25f;
            yield return new WaitForEndOfFrame();
        }
        transform.position = Vector2.zero;
    }

    IEnumerator LevelStartSequence()
    {
        screenBlackout.color = Color.black;
        yield return new WaitForSeconds(0.25f);

        while (screenBlackout.color.a > 0)
        {
            screenBlackout.color = new Color(0, 0, 0, screenBlackout.color.a - 0.075f);
            yield return new WaitForSeconds(0.05f);
        }

        ply.canMove = true;
        string levelName = SceneManager.GetActiveScene().name;

        if (levelName == "level1")
        {
            yield return new WaitForSeconds(1.5f);
            PlayMusic();
            GameObject.Find("DoorOpenAnim").GetComponent<Animator>().Play("DoorOpen");
        }
        else if (levelName == "level2")
        {

        }
        else if (levelName == "level3")
        {

        }
    }

    public void LevelOverSequenece()
    {
        StartCoroutine(LevelOverSequenceCoroutine());
    }

    IEnumerator LevelOverSequenceCoroutine()
    {
        string levelName = SceneManager.GetActiveScene().name;

        if (levelName == "level1")
        {
            yield return new WaitForSeconds(1.5f);
            PlayMusic();
            GameObject.Find("DoorOpenAnim").GetComponent<Animator>().Play("DoorOpen");
        }
        else if (levelName == "level2")
        {

        }
        else if (levelName == "level3")
        {

        }
    }


    public IEnumerator GameOverSequence()
    {
        screenBlackout.rectTransform.anchoredPosition = new Vector2(0, 2000);

        quitBlackout.color = new Color(0, 0, 0, 0.5f);
        gameOverImage.rectTransform.anchoredPosition = Vector2.zero;
        yield return new WaitForSeconds(2);
        Cursor.visible = true;
        gameOverImage.rectTransform.anchoredPosition = new Vector2(0, -2000);
        tryAgainImage.rectTransform.anchoredPosition = Vector2.zero;
        quitYes.anchoredPosition = new Vector2(quitYes.anchoredPosition.x, -256f);
        quitNo.anchoredPosition = new Vector2(quitNo.anchoredPosition.x, -256f);
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(LoadLevelCoroutine(level));
    }

    IEnumerator LoadLevelCoroutine(int level)
    {
        screenBlackout.rectTransform.anchoredPosition = Vector2.zero;
        while (screenBlackout.color.a < 1)
        {
            screenBlackout.color = new Color(0, 0, 0, screenBlackout.color.a + 0.075f);
            sfxSource.volume -= 0.075f;
            yield return new WaitForSeconds(0.05f);
        }
        SceneManager.LoadScene(level);
    }
}
