using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public int levelToLoad;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerController p = FindObjectOfType<PlayerController>();
            p.canMove = false;
            p.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            p.GetComponentInChildren<Animator>().SetFloat("WalkSpeed", 0);
            LoadLevel();
        }
    }

    public void LoadLevel()
    {
        gm.LoadLevel(levelToLoad);
    }

    public void LoadLevelHard()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
