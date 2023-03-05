using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{

    public GameObject sugarPrefab;
    public GameObject spawnPosition;
    public int numberOfSugarToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnFood());
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator SpawnFood()
    {

        int count = 0;
        while (numberOfSugarToSpawn > count)
        {
            GameObject food = Instantiate(sugarPrefab, spawnPosition.transform.position + (Vector3)Random.insideUnitCircle * 0.15f, Quaternion.identity);
            food.transform.parent = this.transform;
            yield return new WaitForSeconds(0.01f);
            count++;
        }
    }
}
