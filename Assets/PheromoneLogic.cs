using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PheromoneType
{
    RED,
    PLAYERCOLOR
}
public class PheromoneLogic : MonoBehaviour
{
    Pheromone pheromone = new Pheromone();

    float creationTime = 0;
    float lifetime = 65f;
    public SpriteRenderer sprite;
    //float pheromoneStrength = 1;
    float initialStrength = 1;
    float evaporateValue;
    public PheromoneType pheromoneType = PheromoneType.PLAYERCOLOR;
    PheromoneController pheromoneController;

    // Start is called before the first frame update
    void StartDecay()
    {
        creationTime = Time.time;
        evaporateValue = 1f / 100f;
       
        StartCoroutine(Decay());
        pheromone.position = this.transform.position;
        pheromoneController.addPheromone(ref this.pheromone, pheromoneType);
    }

    private IEnumerator Decay() {
        float timeSinceCreation = 0;
        while ((timeSinceCreation = Time.time - creationTime) < lifetime) { 
            yield return new WaitForSeconds(Random.Range(1f,3f));
            if (pheromone.strength <= 0)
            {
                //Debug.Log("Phero was deleted");
                break;
            }
            pheromone.strength = initialStrength - evaporateValue * initialStrength * timeSinceCreation;  //initialStrength - initialStrength * lifetime/timeSinceCreation;
        }

        pheromoneController.removePheromone(ref this.pheromone, pheromoneType);
        Destroy(this.gameObject);
    }

    public float getPheromoneStrength() {
        return pheromone.strength;
    }

    public PheromoneType getPheromoneType() {
        return pheromoneType;
    }

    internal void setPheromoneStartStrength(float initialPheromoneStrength, float lifeTime, PheromoneController pheromoneController)
    {
        this.pheromoneController = pheromoneController;
        if (pheromoneType.Equals(PheromoneType.RED))
        {
            if (lifeTime > 25)
            {
                lifeTime = 25;
            }
        }
        else {
            if (lifeTime > 65) {
                lifeTime = 65;
            }
        }
        this.lifetime = lifeTime;


        this.initialStrength = initialPheromoneStrength > 10 ? 10 : (initialPheromoneStrength < 1 ? 1 : initialPheromoneStrength);
        pheromone.strength = this.initialStrength;

        StartDecay();
    }

    internal void setColor(Color playerColor)
    {
        sprite.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.23333f);
    }
}
