using System.Collections;
using UnityEngine;

public class CamMover : MonoBehaviour
{
    public float speed = 1f;
    public float boostAmount = 1f;
    public float boostSpeed = 5f; // How fast the camera surges ahead
    public PlayerController[] players;
    private Vector3 scrollTarget;
    private float boostOffset = 0f;
    SceneMan sceneMan;

    void Start()
    {
        sceneMan = FindFirstObjectByType<SceneMan>();
        scrollTarget = transform.position;
    }

    void Update()
    {
        if(sceneMan.playersAtExit >= 3)
        {
            speed = 0;
        }
        bool anyPlayerMoved = false;
        foreach (PlayerController player in players)
        {
            if (player.firstMove)
            {
                anyPlayerMoved = true;
                break;
            }
        }

        if (!anyPlayerMoved) return; // Don't scroll until a key is pressed

        scrollTarget += Vector3.up * speed * Time.deltaTime;

        boostOffset = Mathf.MoveTowards(boostOffset, 0f, boostSpeed * Time.deltaTime);

        Vector3 finalTarget = scrollTarget + Vector3.up * boostOffset;

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            finalTarget,
            (speed + boostSpeed) * Time.deltaTime
        );

        if (newPosition.y >= transform.position.y)
        {
            transform.position = newPosition;
        }
        else
        {
            // Keep scrollTarget anchored to where the camera actually is
            scrollTarget.y = transform.position.y;
        }
    }

    public void TriggerBoost()
    {
        boostOffset += boostAmount;
    }
}
