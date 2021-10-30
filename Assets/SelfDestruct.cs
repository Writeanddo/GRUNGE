using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timeBeforeDeath;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaitAndDie());
    }

    IEnumerator WaitAndDie()
    {
        yield return new WaitForSeconds(timeBeforeDeath);
        Die();
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
