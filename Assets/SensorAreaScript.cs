using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorAreaScript : MonoBehaviour
{
    
    List<PheromoneLogic> pheromones = new List<PheromoneLogic>();
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("BluePheromone"))
        {
            pheromones.Add(collision.GetComponent<PheromoneLogic>());

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("BluePheromone"))
        {
            pheromones.Remove(collision.GetComponent<PheromoneLogic>());
           
        }
    }
    */
    public float getBluePheromoneStrength() {
        float strength = 0;

        foreach(PheromoneLogic pheromone in pheromones) {
            strength += pheromone.getPheromoneStrength();
        }
        return strength;
    }
    
}
