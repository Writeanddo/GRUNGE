using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableFadeAfterTime : MonoBehaviour
{
    public float timeBeforeFade = 15;

    Grabbable g;
    SpriteRenderer spr;
    IEnumerator FadeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        g = GetComponent<Grabbable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!g.isHeld && FadeCoroutine == null)
        {
            spr.color = Color.white;
            FadeCoroutine = WaitAndFade();
            StartCoroutine(FadeCoroutine);
        }
        else if (g.isHeld && FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
            FadeCoroutine = null;
        }
    }

    IEnumerator WaitAndFade()
    {
        yield return new WaitForSeconds(15);
        while(spr.color.a > 0)
        {
            spr.color = new Color(1, 1, 1, spr.color.a - (Time.fixedDeltaTime / 3));
            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }
}
