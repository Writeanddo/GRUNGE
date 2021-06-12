using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0).GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
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
