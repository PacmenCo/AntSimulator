using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMesh gameTimer;
    public float gameTime;


    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
        float timeLeft = 130f - gameTime;

        if (timeLeft <= 0) {
            Time.timeScale = 0;
        }

        gameTimer.text = "Round Ends in: " + timeLeft.ToString("F1") + "s";
    }
}
