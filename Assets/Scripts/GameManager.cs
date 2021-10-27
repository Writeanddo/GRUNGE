using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using io.newgrounds;

public class GameManager : MonoBehaviour
{
    core ngCore;

    Transform cam;
    CameraController camControl;
    PlayerController ply;
    Slider gooSlider;
    Image gooFill;
    Slider healthSlider;
    Image healthFill;
    Image shieldImage;
    RectTransform shieldTransform;
    
    Image screenBlackout;
    Image quitBlackout;
    Text gooSliderText;
    Text weaponTimerText;
    TextMeshProUGUI killsText;
    TextMeshProUGUI timerText;
    RectTransform levelEndScreen;
    RectTransform replayButton;
    RectTransform quitButton;

    EnemyScript heldEnemy;
    AudioSource musicSource;
    AudioSource musicTrack1;
    AudioSource musicTrack2;
    AudioSource sfxSource;
    AudioSource stoppableSfxSource;
    AudioSource priorityStoppableSfxSource;
    AudioSource gooSource;
    AudioSource prioritySfxSource; // Will duck volume on normal sound effects
    Animator shieldAnim;
    TextMeshProUGUI pauseText;
    Text gunNameText;
    EnemyWaveManager ewm;
    TextboxManager text;
    Animator screenTransition;

    public bool paused;
    public bool canPause = true;
    public bool gameOver;
    public bool playingEndlessMode;

    public AudioClip music;
    public AudioClip endlessMusic;
    public AudioClip[] generalSfx;
    public AudioClip[] playerSfx;
    public AudioClip[] gooPickupSounds;
    public GameObject[] powerups;
    public EnemyWaveManager.EnemyWave[] endlessModeWaves;

    //[HideInInspector]
    public List<EnemyScript> enemiesInLevel;

    public TextAsset dialogSourceFile;

    TextboxManager.TextData[] cachedTextData;

    float gunTimer = 10;
    bool shieldExploding;
    int storedGooAmount;
    int storedHealthAmount;
    int storedSfxPriority;
    Color gooColor;
    Color healthColor;

    int kills = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        cam = transform.GetChild(0).GetChild(0);
        ngCore = GetComponent<core>();
        camControl = FindObjectOfType<CameraController>();
        ply = FindObjectOfType<PlayerController>();
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        healthFill = healthSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        gooSlider = GameObject.Find("GooSlider").GetComponent<Slider>();
        gooFill = gooSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        gooColor = gooFill.color;
        healthColor = healthFill.color;
        sfxSource = GameObject.Find("GameSFX").GetComponent<AudioSource>();
        stoppableSfxSource = GameObject.Find("GameStoppableSFX").GetComponent<AudioSource>();
        musicSource = GameObject.Find("GameMusic").GetComponent<AudioSource>();
        musicTrack1 = GameObject.Find("GameMusicTrackDrums").GetComponent<AudioSource>();
        musicTrack2 = GameObject.Find("GameMusicTrackInstruments").GetComponent<AudioSource>();
        gooSource = GameObject.Find("GameGooPickupSFX").GetComponent<AudioSource>();
        prioritySfxSource = GameObject.Find("GamePrioritySFX").GetComponent<AudioSource>();
        priorityStoppableSfxSource = GameObject.Find("GamePriorityStoppableSFX").GetComponent<AudioSource>();
        shieldAnim = GameObject.Find("ShieldImage").GetComponent<Animator>();
        shieldTransform = shieldAnim.GetComponent<RectTransform>();
        gooSliderText = GameObject.Find("GooSliderNumber").GetComponent<Text>();
        killsText = GameObject.Find("KillsNumberText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("LevelTimeText").GetComponent<TextMeshProUGUI>();
        replayButton = GameObject.Find("QuitYesButton").GetComponent<RectTransform>();
        quitButton = GameObject.Find("QuitNoButton").GetComponent<RectTransform>();

        screenBlackout = GameObject.Find("ScreenBlackout").GetComponent<Image>();
        quitBlackout = GameObject.Find("QuitPanel").GetComponent<Image>();
        levelEndScreen = GameObject.Find("LevelStatsLayout").GetComponent<RectTransform>();
        pauseText = GameObject.Find("PausedText").GetComponent<TextMeshProUGUI>();
        weaponTimerText = GameObject.Find("WeaponTimerText").GetComponent<Text>();
        gunNameText = GameObject.Find("WeaponNameText").GetComponent<Text>();
        gunNameText.text = "";
        weaponTimerText.text = "";
        shieldImage = GameObject.Find("ShieldImage").GetComponent<Image>();
        screenTransition = GameObject.Find("ScreenTransition").GetComponent<Animator>();

        ewm = FindObjectOfType<EnemyWaveManager>();
        text = FindObjectOfType<TextboxManager>();

        if (dialogSourceFile != null)
            cachedTextData = JsonHelper.FromJson<TextboxManager.TextData>(dialogSourceFile.text);

        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            int endless = PlayerPrefs.GetInt("GRUNGE_IS_ENDLESS");
            if (endless == 1)
                playingEndlessMode = true;
            else
                playingEndlessMode = false;
        }       
        enemiesInLevel = new List<EnemyScript>();
        StartCoroutine(LevelStartSequence());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUI();
        enemiesInLevel.RemoveAll(item => item == null);

        if (!gameOver && ewm.isSpawningEnemies && !paused)
            timer += Time.fixedDeltaTime;
    }

    public void IncreaseKills()
    {
        kills++;
    }

    // Basement time: 10987
    // Basement kills: 10986
    // Front lawn time: 10982
    // Front lawn kills: 10983
    // House time: 10985
    // House kills: 10984
    // Overall kills: 10980
    int GetIdFromLevelIndex(int index, bool isTime)
    {
        int[] timeIds = new int[3] { 10982, 10985, 10987 };
        int[] killIds = new int[3] { 10983, 10984, 10986};

        if (isTime)
            return timeIds[index - 3];
        else
            return killIds[index - 3];
    }

    public void PostScore(int id, int value)
    {
        io.newgrounds.components.ScoreBoard.postScore scoreToPost = new io.newgrounds.components.ScoreBoard.postScore();
        scoreToPost.id = id;
        scoreToPost.value = value;
        scoreToPost.callWith(ngCore);
        print("Posted score " + id);
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
            {
                if (ply.stats.goo >= 30 && storedGooAmount < 30)
                    PlayPrioritySFX(playerSfx[6]);
                if (ply.stats.goo >= 50 && storedGooAmount < 50)
                    PlaySFX(playerSfx[7]);

                gooFill.color = new Color(0, 0.75f, 0);
            }
            else if (ply.stats.goo < storedGooAmount)
            {
                gooFill.color = new Color(0.75f, 0.75f, 0);
            }
            storedGooAmount = ply.stats.goo;
            gooSliderText.text = ply.stats.goo.ToString();

            // Shield update
            shieldImage.rectTransform.localScale = Vector2.Lerp(shieldImage.rectTransform.localScale, Vector2.one, 0.25f);
            if (ply.heldObject != null && ply.heldObject.tag == "Enemy")
            {
                if (heldEnemy == null)
                {
                    heldEnemy = ply.heldObject.GetComponent<EnemyScript>();
                    shieldImage.rectTransform.localScale = new Vector2(1.25f, 1.25f);
                    shieldExploding = false;
                }

                CheckAndPlayClip("Shield_" + heldEnemy.stats.currentShieldValue, shieldAnim);
                Vector2 offset = ply.transform.position - cam.transform.position;
                shieldTransform.anchoredPosition = new Vector2(offset.x * 32, 64 + offset.y * 32);
            }
            else if (heldEnemy != null && !shieldExploding)
            {
                heldEnemy = null;
                CheckAndPlayClip("Shield_Empty", shieldAnim);
            }

            // Weapon timer update
            if (ply.stats.currentWeapon != 0 && ply.stats.currentWeapon != 10)
            {
                gunTimer -= Time.fixedDeltaTime;
                weaponTimerText.text = Mathf.RoundToInt(gunTimer).ToString();

                if (gunTimer <= 0)
                {
                    ply.stats.currentWeapon = 0;
                    weaponTimerText.text = "";
                    gunNameText.text = "";
                }
            }

        }

        // Pause game
        if (paused && pauseText.text == "" && canPause)
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

    public IEnumerator WaitForTextCompletion(string id)
    {
        while (text.isPrinting)
            yield return null;

        yield return text.PrintSingleText(GetTextDataFromID(id));
    }

    TextboxManager.TextData GetTextDataFromID(string id)
    {
        foreach (TextboxManager.TextData t in cachedTextData)
        {
            if (t.id == id)
                return t;
        }
        return null;
    }

    public void ShieldHit()
    {
        // Scale up the shield icon (visual effect only)
        shieldImage.rectTransform.localScale = new Vector2(1.25f, 1.25f);
    }

    public void ShieldBreak()
    {
        shieldExploding = true;
        shieldImage.rectTransform.localScale = new Vector2(1.25f, 1.25f);
        CheckAndPlayClip("Shield_Break", shieldAnim);
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicTrack1.Stop();
        musicTrack2.Stop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && canPause)
        {
            paused = !paused;
            Cursor.visible = !paused;
            if (!paused)
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
                Cursor.lockState = CursorLockMode.None;
        }
    }


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.pitch = 1;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float pitch)
    {
        //sfxSource.Stop();
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFXStoppable(AudioClip clip, float pitch)
    {
        stoppableSfxSource.Stop();
        stoppableSfxSource.pitch = pitch;
        stoppableSfxSource.PlayOneShot(clip);
    }

    public void PlaySFXStoppablePriority(AudioClip clip, float pitch)
    {
        priorityStoppableSfxSource.Stop();
        priorityStoppableSfxSource.pitch = pitch;
        priorityStoppableSfxSource.PlayOneShot(clip);
    }

    public void StopPrioritySFX()
    {
        priorityStoppableSfxSource.Stop();
    }

    public void PlayGooSFX(AudioClip clip)
    {
        gooSource.Stop();
        gooSource.pitch = 0.75f + (ply.stats.goo / (float)ply.stats.maxGoo) * 0.75f;
        gooSource.PlayOneShot(clip);
    }

    public void PlayPrioritySFX(AudioClip clip)
    {
        sfxSource.pitch = 1;
        prioritySfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        if (playingEndlessMode)
            musicSource.clip = endlessMusic;
        else
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

    IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        while (duration > 0)
        {
            cam.localPosition = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) * intensity;
            if (duration < Time.fixedDeltaTime * 10)
                intensity /= 1.25f;

            duration -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        //transform.position = Vector2.zero;
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

        string levelName = SceneManager.GetActiveScene().name;

        if (levelName == "0_tutorial")
        {
            PlayMusic();
            StartCoroutine(FindObjectOfType<TutorialManager>().IntroDialog());
        }
        else if (levelName == "1_cabin_approach")
        {
            canPause = false;
        }
        else if (levelName == "2_cabin_interior")
        {
            ply.canMove = true;
            //PlayMusic();
            //ewm.StartWaves();
        }
        else if (levelName == "3_basement")
        {
            ply.canMove = true;
            PlayMusic();
            ewm.StartWaves();
        }
        else if (levelName == "4_boss")
        {
            canPause = false;
            PlayMusic();
            yield return new WaitForSeconds(1f);
            camControl.overridePosition = true;
            Transform t = GameObject.Find("Boss").transform;

            ply.canMove = false;
            ply.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            float timer = 1f;
            while (timer > 0)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(t.position.x, t.position.y, camControl.transform.position.z), 0.1f);
                timer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            PlaySFX(generalSfx[6]);
            t.GetComponent<Animator>().Play("BossRoar");
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(ScreenShakeCoroutine(2, 4.5f));
            yield return new WaitForSeconds(2f);

            cam.localPosition = Vector2.zero;

            yield return new WaitForSeconds(3f);

            camControl.secondTarget = t;
            yield return new WaitForEndOfFrame();
            camControl.overridePosition = false;
            transform.position = Vector2.zero;

            t.GetComponent<EnemyScript>().UpdateMovement();
            ewm.StartWaves();
            canPause = true;
            ply.canMove = true;
        }
        else
            ply.canMove = true;
    }

    public void LevelOverSequenece()
    {
        StartCoroutine(LevelOverSequenceCoroutine());
    }

    public IEnumerator BossPhase1DieCutscene()
    {
        canPause = false;
        StopMusic();
        ewm.ForceStopSpawningEnemies();

        // Destroy all other enemies and projectiles
        EnemyScript[] enemies = FindObjectsOfType<EnemyScript>();
        EnemyProjectile[] projectiles = FindObjectsOfType<EnemyProjectile>();
        BossIdolScript idol = FindObjectOfType<BossIdolScript>();

        if (idol != null)
            Destroy(idol.gameObject);
        for (int i = 0; i < enemies.Length; i++)
            if (enemies[i].name != "Boss")
                Destroy(enemies[i].gameObject);
        for (int i = 0; i < projectiles.Length; i++)
            Destroy(projectiles[i].gameObject);

        camControl.overridePosition = true;
        Transform t = GameObject.Find("Boss").transform;
        ply.Freeze();
        ply.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        BossEnemyScript b = t.GetComponent<BossEnemyScript>();
        Instantiate(b.enemyExplosion, new Vector2(t.position.x + Random.Range(-1f, 1f) * 2, t.position.y + Random.Range(-1f, 1f) * 2), Quaternion.identity);

        float timer = 0.5f;
        while (timer > 0)
        {
            camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(t.position.x, t.position.y + 2, camControl.transform.position.z), 0.2f);
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        PlaySFX(generalSfx[7]);
        //StartCoroutine(LoadVictoryLevel());
        for (int i = 2; i < 15; i++)
        {
            Instantiate(b.enemyExplosion, new Vector2(t.position.x + Random.Range(-1f, 1f) * 2, t.position.y + Random.Range(-1f, 1f) * 2), Quaternion.identity);
            yield return new WaitForSeconds(1f / (i * 1.5f));
        }
        Instantiate(b.bigExplosion, new Vector2(t.position.x, t.position.y), Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        b.GetComponent<Animator>().Play("BossDead");

        // Transformation begins here
        SpriteRenderer bossHeart = GameObject.Find("BossHeart").GetComponent<SpriteRenderer>();
        bossHeart.color = Color.white;
        yield return new WaitForSeconds(3f);
        StartCoroutine(FindObjectOfType<BossPhase2Manager>().PlayTransformationCutscene());
    }

    public void PickupGun(int gunIndex)
    {
        PlaySFX(generalSfx[12]);
        ply.stats.currentWeapon = gunIndex;
        switch (gunIndex)
        {
            case 10:
                gunNameText.text = "THRESH";
                gunTimer = 20;
                ply.canLaunchHand = false;
                break;

            case 1:
                gunNameText.text = "SHOT\nGUN";
                gunTimer = 15;
                break;
            case 2:
                gunNameText.text = "TOMMY\nGUN";
                gunTimer = 15;
                break;
            case 11:
                gunNameText.text = "BRASS\nKNUCKLES";
                gunTimer = 15;
                ply.canLaunchHand = false;
                break;
        }
    }

    IEnumerator LevelOverSequenceCoroutine()
    {
        string levelName = SceneManager.GetActiveScene().name;

        if (levelName == "1_cabin_approach")
        {
            camControl.overridePosition = true;
            Transform t = GameObject.Find("CabinDoorHole").transform;

            ply.canMove = false;
            ply.GetComponentInChildren<Animator>().Play("Player_FaceS");
            ply.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            StartCoroutine(ReverseFadeMusic());


            float timer = 2.25f;
            while (timer > 0)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(t.position.x, t.position.y, camControl.transform.position.z), 0.1f);
                timer -= Time.deltaTime;
                yield return null;
            }

            while (Vector3.Distance(camControl.transform.position, ply.transform.position) > 10.1f)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(ply.transform.position.x, ply.transform.position.y, camControl.transform.position.z), 0.1f);
                yield return null;
            }

            GameObject.Find("LevelLoader").transform.position = new Vector3(9, 21.5f, 0);

            UpdateUnlockedLevels(2);

            camControl.overridePosition = false;
            ply.canMove = true;
        }
        else if (levelName == "2_cabin_interior")
        {
            camControl.overridePosition = true;
            Transform t = GameObject.Find("TrapdoorOpen").transform;
            t.GetComponent<Animator>().Play("TrapdoorOpen");

            ply.canMove = false;
            ply.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            float timer = 2.25f;
            while (timer > 0)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(t.position.x, t.position.y, camControl.transform.position.z), 0.1f);
                timer -= Time.deltaTime;
                yield return null;
            }

            while (Vector3.Distance(camControl.transform.position, ply.transform.position) > 10.1f)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(ply.transform.position.x, ply.transform.position.y, camControl.transform.position.z), 0.1f);
                yield return null;
            }

            camControl.overridePosition = false;
            ply.canMove = true;

            UpdateUnlockedLevels(3);
        }
        else if (levelName == "3_basement")
        {
            camControl.overridePosition = true;
            Transform t = GameObject.Find("GoodoorOpen").transform;
            t.GetComponent<Animator>().Play("GoodoorOpen");

            ply.canMove = false;
            ply.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            float timer = 2.25f;
            while (timer > 0)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(t.position.x, t.position.y, camControl.transform.position.z), 0.1f);
                timer -= Time.deltaTime;
                yield return null;
            }

            while (Vector3.Distance(camControl.transform.position, ply.transform.position) > 10.1f)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(ply.transform.position.x, ply.transform.position.y, camControl.transform.position.z), 0.1f);
                yield return null;
            }

            camControl.overridePosition = false;


            yield return WaitForTextCompletion("PlaytestOver");
            ply.canMove = true;
            //UpdateUnlockedLevels(4);
        }
        else if (levelName == "4_boss")
        {
            StopMusic();

            ply.stats.health = 1000;
            ply.stats.maxHealth = 1000;

            ply.canLaunchHand = false;
            camControl.overridePosition = true;
            ply.transform.position = new Vector2(0, 200);
            Transform t = GameObject.Find("Boss").transform;
            ply.canMove = false;
            ply.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            float timer = 0.25f;
            while (timer > 0)
            {
                camControl.transform.position = Vector3.Lerp(camControl.transform.position, new Vector3(t.position.x, t.position.y, camControl.transform.position.z), 0.5f);
                timer -= Time.deltaTime;
                yield return null;
            }

            PlaySFX(generalSfx[7]);
            StartCoroutine(LoadVictoryLevel());
            GameObject g = t.GetComponent<EnemyScript>().enemyExplosion;
            for (int i = 0; i < 30; i++)
            {
                Instantiate(g, new Vector2(t.position.x + Random.Range(-1f, 1f) * 2, t.position.y + Random.Range(-1f, 1f) * 2), Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }
        }

        PlayerPrefs.Save();
    }

    void UpdateUnlockedLevels(int level)
    {
        if (PlayerPrefs.GetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL") < level)
            PlayerPrefs.SetInt("GRUNGE_FURTHEST_UNLOCKED_LEVEL", level);

    }

    public IEnumerator GameOverSequence()
    {
        ShowResultsScreen(true);
        yield return null;
    }

    public void LevelCompleteSequence()
    {
        StopMusic();
        ShowResultsScreen(false);
    }

    void ShowResultsScreen(bool isGameOver)
    {
        // Submit to scoreboard if we're playing endless mode
        if(playingEndlessMode && isGameOver)
        {
            PostScore(GetIdFromLevelIndex(SceneManager.GetActiveScene().buildIndex, false), kills);
            PostScore(GetIdFromLevelIndex(SceneManager.GetActiveScene().buildIndex, true), Mathf.RoundToInt(timer * 1000));
        }

        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        int milliseconds = Mathf.FloorToInt(((timer - (minutes * 60) - seconds)) * 100);
        string niceTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        timerText.text = niceTime;

        killsText.text = kills.ToString();

        PlaySFX(generalSfx[19]);
        screenBlackout.rectTransform.anchoredPosition = new Vector2(0, 2000);
        quitBlackout.color = new Color(0, 0, 0, 0.5f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(LerpResultsScreen(isGameOver));
    }

    IEnumerator LerpResultsScreen(bool isGameOver)
    {
        while (Vector2.Distance(levelEndScreen.anchoredPosition, Vector2.zero) > 1)
        {
            levelEndScreen.anchoredPosition = Vector2.Lerp(levelEndScreen.anchoredPosition, Vector2.zero, 0.25f);
            yield return new WaitForFixedUpdate();
        }

        if(isGameOver)
        {
            replayButton.anchoredPosition = new Vector2(-88, -240);
            quitButton.anchoredPosition = new Vector2(84, -240);
        }

        levelEndScreen.anchoredPosition = Vector2.zero;
    }

    public void LoadLevel(int level)
    {
        PlayerPrefs.SetInt("GRUNGE_LOAD_TO_LEVEL_SELECT", 1);
        PlayerPrefs.Save();
        StartCoroutine(LoadLevelCoroutine(level));
    }

    public void ReloadLevel()
    {
        int loadedLevel = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadLevelCoroutine(loadedLevel));
    }

    IEnumerator LoadLevelCoroutine(int level)
    {
        screenBlackout.rectTransform.anchoredPosition = Vector2.zero;
        PlaySFX(generalSfx[3]);
        screenTransition.Play("ScreenTransition", -1, 0);
        yield return new WaitForSeconds(0.25f);
        screenBlackout.color = Color.black;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(level);
    }

    public IEnumerator FadeToBlack()
    {
        screenBlackout.rectTransform.anchoredPosition = Vector2.zero;
        while (screenBlackout.color.a < 1)
        {
            screenBlackout.color = new Color(0, 0, 0, screenBlackout.color.a + 0.075f);
            sfxSource.volume -= 0.075f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator FadeFromBlack()
    {
        screenBlackout.rectTransform.anchoredPosition = Vector2.zero;
        while (screenBlackout.color.a > 0)
        {
            screenBlackout.color = new Color(0, 0, 0, screenBlackout.color.a - 0.075f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator LoadVictoryLevel()
    {
        int level = 4;
        screenBlackout.rectTransform.anchoredPosition = Vector2.zero;
        while (screenBlackout.color.a < 1)
        {
            screenBlackout.color = new Color(1, 1, 1, screenBlackout.color.a + 0.01f);
            sfxSource.volume -= 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(level);
    }

    public IEnumerator FadeMusic()
    {
        while (musicTrack1.volume > 0)
        {
            musicTrack2.volume = Mathf.Clamp(musicTrack2.volume + Time.fixedDeltaTime * 2, 0, 1);
            musicTrack1.volume -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        musicTrack1.volume = 0;
        musicTrack2.volume = 1;

    }

    public IEnumerator ReverseFadeMusic()
    {
        while (musicTrack2.volume > 0)
        {
            musicTrack1.volume = Mathf.Clamp(musicTrack1.volume + Time.fixedDeltaTime * 2, 0, 1);
            musicTrack2.volume -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        musicTrack1.volume = 1;
        musicTrack2.volume = 0;
    }

    public void CheckAndPlayClip(string clipName, Animator anim)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(clipName))
        {
            anim.Play(clipName);
        }
    }
}
