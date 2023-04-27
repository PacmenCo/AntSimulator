using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLogic : MonoBehaviour
{

    //OBS: Værdierne for public vars, kan findes på "Ant" opjektet i editoren!
    public PheromoneController pheromoneController;
    public AntennaeLogic leftAntennae, rightAntennae;
    public GameObject sensorPositionLeft, sensorPositionRight;
    public SpriteRenderer playerColorIndicator;
    public Vector3 antHillPosition;

    public float maxTurnStrength;
    float turnStrength;
    public float maxSpeed;
    public float randomMovementFactor = 0.4f;

    Vector2 position;
    Vector2 velocity;
    Vector2 desiredDirection; 
    Rigidbody2D rigidbody2D;
    bool carryingSugar = false;
    public Vector2 startPosition;

    private SugarLogic targetFood;

    int ticks = 100;
    float bluePheromoneStrengthLeft;
    float bluePheromoneStrengthRight;
    float redPheromoneStrengthLeft;
    float redPheromoneStrengthRight;

    float relativeRatio;

    Vector2 steeringForce;
    Vector2 acceleration;
    float angle;
    public AntController antController;

    public GameObject bluePheromonePrefab, redPheromonePrefab;

    float timePastSinceLastPheromoneTrail = 0;
    float timeBetweenPheromonePlacement = 0.43f;

    internal AnthillScript antHillScript;



    void Start()
    {
        position = this.transform.position;
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        if (!this.GetComponent<AntController>()) { 
            this.gameObject.SetActive(false);
        }
        antController = this.GetComponent<AntController>();
    }

    internal void setColor(Color color)
    {
        playerColorIndicator.color = color;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Food"))
        {
            antController.FoodLocated(other.gameObject.GetComponent<SugarLogic>());
        }
    }

    float timeWhenLastPheromoneWasPlaced = -10f;

    public void placePheromone(PheromoneType type, float initialPheromoneStrength, float lifetime)
    {
        if (Time.time - timeWhenLastPheromoneWasPlaced > timeBetweenPheromonePlacement)
        {
            timeWhenLastPheromoneWasPlaced = Time.time;
            GameObject pheromone;
            if (type == PheromoneType.RED)
            {
                pheromone = Instantiate(redPheromonePrefab, this.gameObject.transform.position + new Vector3(0,0,0.01f), Quaternion.identity, null);
            }
            else
            {
                pheromone = Instantiate(bluePheromonePrefab, this.gameObject.transform.position + 2 * new Vector3(0, 0, 0.01f), Quaternion.identity, null);
                pheromone.GetComponent<PheromoneLogic>().setColor(playerColorIndicator.color);
            }
            pheromone.GetComponent<PheromoneLogic>().setPheromoneStartStrength(initialPheromoneStrength, lifetime, pheromoneController);
        }

    }

    internal void setTurnStrength(float newTurnStrength)
    {
        if (newTurnStrength > maxTurnStrength) {
            newTurnStrength = maxTurnStrength;
        }
        turnStrength = newTurnStrength;
    }

    internal float getTurnStrength()
    {
        return turnStrength;
    }

    internal void LocatedFood(SugarLogic food)
    {
        if (targetFood == null && !food.aquireAsTarget())
        {
            this.targetFood = food;
        }
    }

   

    void FixedUpdate()
    {
        float speed = maxSpeed * (isCarryingSugar() ? 0.65f : 1);
        ticks++;
        turnStrength = maxTurnStrength;
        Vector2 desiredVelocityVector;
        float antennaTurnDegree = 50;
        float deltaTime = Time.fixedDeltaTime;

        //Hvis myren rammer en væg med begge antenner vil den tabe meget hastighed
        if (leftAntennae.isColliding && rightAntennae.isColliding) {
            velocity *= 0.47f;
        }
        //Hvis når myren rammer en væg vil den ændre kurs meget drastisk
        if (leftAntennae.isColliding)
        {
            desiredDirection = new Vector2(Mathf.Cos((getCurrentRotation() - antennaTurnDegree) * Mathf.Deg2Rad), Mathf.Sin((getCurrentRotation() - antennaTurnDegree) * Mathf.Deg2Rad)).normalized;
            velocity *= 0.97f;
            turnStrength *= 10;
        }
        else if (rightAntennae.isColliding)
        {
            desiredDirection = new Vector2(Mathf.Cos((getCurrentRotation() + antennaTurnDegree) * Mathf.Deg2Rad), Mathf.Sin((getCurrentRotation() + antennaTurnDegree) * Mathf.Deg2Rad)).normalized;
            velocity *= 0.97f;
            turnStrength *= 10;
        }
        else
        {
            desiredDirection = antController.ControlAnt(deltaTime).normalized;  //kode hvor du har kontrol over hvad myren skal gøre

            if (turnStrength > maxTurnStrength)
            {
                turnStrength = maxTurnStrength;
            }
            if (speed > maxSpeed) {
                speed = maxSpeed;
            }
        }
        desiredVelocityVector = desiredDirection * speed;
        steeringForce = (desiredVelocityVector - velocity) * turnStrength;
        acceleration = Vector2.ClampMagnitude(steeringForce, turnStrength);

        velocity = Vector2.ClampMagnitude(velocity + acceleration * deltaTime, maxSpeed);

        position += velocity * deltaTime;

        rigidbody2D.MovePosition(position);

        angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        this.gameObject.transform.SetPositionAndRotation(this.gameObject.transform.position, Quaternion.Euler(0,0, angle));

    }

    internal bool attemptToDepositSugar(SugarLogic targetSugar)
    {
        if (carryingSugar && Vector2.Distance(antHillPosition, this.transform.position) < 0.35f)
        {
            carryingSugar = false;
            targetSugar.gameObject.SetActive(false); //DESTROY????????????
            //velocity *= 0.05f;
            antHillScript.UpdateScore();
            return true;
        }
        return false;
    }

    internal Vector3 getAntHillPosition() {
        return antHillPosition;
    }

    internal void slowAntByFactor(float factor)
    {
        if (factor < 1f)
        {
            velocity *= factor;
        }
    }

    private bool canControlAnt()
    {
        return redPheromoneStrengthLeft + redPheromoneStrengthRight > 0.1 || carryingSugar;
    }

    public float getCurrentRotation() {
        return transform.eulerAngles.z;
    }

    internal bool attemptToPickUpSugar(SugarLogic targetSugar)
    {
        if (targetSugar == null) {
            return false;
        }
        if (Vector2.Distance(targetSugar.gameObject.transform.position, this.transform.position) < 0.25f)
        {
            //Debug.Log("Sugar was picked up");
            targetSugar.pickUp();
            targetSugar.gameObject.transform.parent = this.transform;
            carryingSugar = true;
            //velocity *= 0.1f;
            return true;
        }
        return false;
    }

    internal void SensePheromones(PheromoneType pheromoneType)
    {
        if (ticks > 1)
        {
            ticks = 0;
            if (pheromoneType.Equals(PheromoneType.PLAYERCOLOR))
            {
                bluePheromoneStrengthLeft = pheromoneController.getPheromoneStrength(sensorPositionLeft.transform.position, PheromoneType.PLAYERCOLOR);
                bluePheromoneStrengthRight = pheromoneController.getPheromoneStrength(sensorPositionRight.transform.position, PheromoneType.PLAYERCOLOR);
                redPheromoneStrengthLeft = 0;
                redPheromoneStrengthRight = 0;
            }
            else
            {
                bluePheromoneStrengthLeft = 0;
                bluePheromoneStrengthRight = 0;
                redPheromoneStrengthLeft = pheromoneController.getPheromoneStrength(sensorPositionLeft.transform.position, PheromoneType.RED);
                redPheromoneStrengthRight = pheromoneController.getPheromoneStrength(sensorPositionRight.transform.position, PheromoneType.RED);
            }
        }
    }

    internal float getBluePheromoneStrengthLeft()
    {
        return bluePheromoneStrengthLeft;
    }

    internal float getBluePheromoneStrengthRight()
    {
        return bluePheromoneStrengthRight;
    }

    internal float getRedPheromoneStrengthLeft()
    {
        return redPheromoneStrengthLeft;
    }

    internal float getRedPheromoneStrengthRight()
    {
        return redPheromoneStrengthRight;
    }

    internal Vector2 getDesiredDirection() {
        return desiredDirection;
    }
/*
    internal void setDesiredDirection(Vector2 newDesiredDirection)
    {
        desiredDirection = newDesiredDirection.normalized;
    }
*/
    public bool isCarryingSugar() {
        return carryingSugar;
    }

    internal Vector2 getDirectionToAnthill()
    {
        if (Vector2.Distance(getAntHillPosition(), this.transform.position) < 1.5f)
        {
            return (getAntHillPosition() - this.transform.position).normalized;
        }
        return Vector2.zero;
    }
}
