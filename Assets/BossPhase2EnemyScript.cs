using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase2EnemyScript : EnemyScript
{
    public GameObject[] projectiles;
    float movementTime;
    float projectileAngleOffset;

    public float lastFrameXPos;
    public float lastFrameYPos;
    float maxPosChange = 0.2f;

    int currentAttack;
    string animationSuffix = "Healthy";

    Vector3 initialPosition;
    Vector3 headHoleOffset = Vector3.up * 1.75f;

    IEnumerator combatCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        initialPosition = transform.position;
        combatCoroutine = CombatLoop();
        StartCoroutine(combatCoroutine);
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    IEnumerator CombatLoop()
    {
        // Attacks:
        // 0 - Bullet-firing laser
        // 1 - Scum Skull circle pattern
        // 2 - Slow moving snot bubbles
        currentAttack = 1;//Random.Range(0, 3);

        switch(currentAttack)
        {
            case 0:
                break;
            case 1:
                yield return GhostBulletsAttack();
                break;
            case 2:
                break;
        }

        yield return null;
    }

    IEnumerator LaserAttack()
    {
        yield return null;
    }

    IEnumerator GhostBulletsAttack()
    {
        for(int i = 0; i < 5; i++)
        {
            if ((i + 1) % 2 == 0)
                projectileAngleOffset = Mathf.PI / 2;
            else
                projectileAngleOffset = 0;

            anim.Play("BossShoot_" + animationSuffix, -1, 0);
            yield return new WaitForSeconds(5f);
        }
        yield return null;
    }

    IEnumerator SnotBallAttack()
    {
        yield return null;
    }


    float MCos(float value)
    {
        return Mathf.Cos(Mathf.Deg2Rad * value);
    }

    float MSin(float value)
    {
        return Mathf.Sin(Mathf.Deg2Rad * value);
    }

    public override void UpdateMovement()
    {
        return;

        movementTime += Time.fixedDeltaTime;

        if (movementTime > 2 * Mathf.PI)
            movementTime -= 2 * Mathf.PI;

        float xPos = Mathf.Sin(movementTime) * 15;
        float yPos = Mathf.Sin(movementTime * 2) * 4;

        float xDiff = Mathf.Abs(lastFrameXPos - xPos);
        float yDiff = Mathf.Abs(lastFrameYPos - yPos);

        print(xDiff + ", " + yDiff);
        if (xDiff > maxPosChange)
            xPos = lastFrameXPos + (maxPosChange * Mathf.Sign(lastFrameXPos));

        lastFrameXPos = xPos;

        if (yDiff > maxPosChange)
            yPos = lastFrameYPos + (maxPosChange * Mathf.Sign(lastFrameYPos));

        lastFrameYPos = yPos;

        //transform.RotateAround(ply.transform.position, transform.forward, 1);
        transform.rotation = Quaternion.identity;
        transform.position = initialPosition + new Vector3(xPos, yPos);
    }

    public void SpawnCurrentProjectiles()
    {
        StartCoroutine(SpawnCurrentProjectilesCoroutine());
    }

    IEnumerator SpawnCurrentProjectilesCoroutine()
    {
        if (currentAttack == 1)
        {
            int sides = 32;
            for (int i = 0; i < sides; i++)
            {
                ScumSkullEnemyScript s = Instantiate(projectiles[1], transform.position + headHoleOffset, Quaternion.identity).GetComponent<ScumSkullEnemyScript>();
                float val = (i / (float)sides * 2*Mathf.PI) + projectileAngleOffset;
                print(val);
                s.targetVelocity = new Vector2(Mathf.Cos(val), Mathf.Sin(val)).normalized * 4;
                yield return new WaitForSeconds(0.05f);
                //gm.PlaySFXStoppable(gm.generalSfx[3], 1);
            }
        }
    }
}
