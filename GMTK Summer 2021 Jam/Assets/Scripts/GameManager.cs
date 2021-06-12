using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Transform cam;
    PlayerController ply;
    Slider gooSlider;
    Image gooFill;
    Slider healthSlider;
    Image healthFill;

    int storedGooAmount;
    int storedHealthAmount;
    Color gooColor;

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
        healthFill.color = Color.Lerp(healthFill.color, Color.red, 0.1f);
        if (ply.stats.health > storedHealthAmount)
            healthFill.color = new Color(1, 0, 1);
        else if (ply.stats.health < storedHealthAmount)
            healthFill.color = new Color(0.5f, 0, 0);
        storedHealthAmount = ply.stats.health;

        // Goo update
        gooSlider.value = Mathf.Lerp(gooSlider.value, (float)ply.stats.goo / ply.stats.maxGoo, 0.1f);
        gooFill.color = Color.Lerp(gooFill.color, gooColor, 0.1f);
        if (ply.stats.goo > storedGooAmount)
            gooFill.color = new Color(0, 0.75f, 0);
        else if(ply.stats.goo < storedGooAmount)
            gooFill.color = new Color(0.75f, 0.75f, 0);
        storedGooAmount = ply.stats.goo;
    }


    public void PlaySFX(AudioClip clip)
    {
        // todo
    }


    public void ScreenShake(float intensity)
    {
        StartCoroutine(ScreenShakeCoroutine(intensity));
    }

    IEnumerator ScreenShakeCoroutine(float intensity)
    {
        for(int i = 0; i < 10; i++)
        {
            cam.localPosition = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) * intensity;
            intensity /= 1.25f;
            yield return new WaitForEndOfFrame();
        }
        transform.position = Vector2.zero;
    }
}
