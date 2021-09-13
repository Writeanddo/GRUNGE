using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenCutscene : MonoBehaviour
{
    public GameObject wallToDisable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ExplodeWall()
    {
        wallToDisable.SetActive(false);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            GetComponent<Animator>().Play("DoorOpen");
    }
}
