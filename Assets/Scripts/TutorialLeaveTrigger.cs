using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLeaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StartCoroutine(WaitForDialog());
    }

    IEnumerator WaitForDialog()
    {
        PlayerController ply = FindObjectOfType<PlayerController>();
        ply.Freeze();
        yield return FindObjectOfType<GameManager>().WaitForTextCompletion("TutorialLeaveEarly");
        ply.canMove = true;
        Destroy(this.gameObject);
    }
}
