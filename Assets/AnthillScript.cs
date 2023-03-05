using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnthillScript : MonoBehaviour
{
    public GameObject antHill;
    public int numberOfAntsToSpawn;
    public GameObject antPrefab;
    public GameObject spawnPosition;
    public PheromoneController pheromoneController;
    public TextMesh scoreText;
    public static int playerCount = 0;
    private int playerNumber;
    int score = 0;
    private string playerName = null;
 

    // Start is called before the first frame update
    void Start()
    {
        playerCount++;
        playerNumber = playerCount;

        antHill = spawnPosition;
        StartCoroutine(SpawnAnts());
    }

    IEnumerator SpawnAnts() {
        int numberofAntsSpawned = 0;
        while (numberOfAntsToSpawn > numberofAntsSpawned) {
            SpawnAnt();
            yield return new WaitForSeconds(0.45f*(float)numberofAntsSpawned / (float)numberOfAntsToSpawn);
            numberofAntsSpawned += 2;
        }
    }

    public void SpawnAnt() {
        GameObject ant = Instantiate(antPrefab, spawnPosition.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
        ant.GetComponent<AntLogic>().pheromoneController = pheromoneController;
        ant.transform.parent = this.transform;
        ant.GetComponent<AntLogic>().setColor(GetPlayerColor());
        ant.GetComponent<AntLogic>().antHillPosition = antHill.transform.position;
        ant.GetComponent<AntLogic>().antHillScript = this;

        if (playerName == null) {
            string name = ant.GetComponent<AntController>().GetPlayerName();
            scoreText.color = GetPlayerColor();
            if (name.Length > 12)
            {
                playerName = ant.GetComponent<AntController>().GetPlayerName().Substring(0, 12);
            }
            else {
                playerName = name;
            }
            UpdateUI();
        }
    }

    private Color GetPlayerColor()
    {
        if (playerNumber == 1) { 
            return Color.white;
        }
        if (playerNumber == 2)
        {
            return Color.cyan;
        }
        if (playerNumber == 3)
        {
            return Color.green;
        }
        if (playerNumber == 4)
        {
            return Color.yellow;
        }

        return Color.white;
    }

    internal void UpdateScore()
    {
        score += 1;
        UpdateUI();
      
    }

    private void UpdateUI() {
        scoreText.text = playerName + System.Environment.NewLine +
                           "Score: " + score;
    }
}
