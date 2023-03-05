using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//RENAME GERNE DENNE KLASSE - Skal være forskelligt per spiller - Du skal også ændre klassens navn på filen før det fungerer.
//derefter skal filen dragges over på "Ant"-prefabben 
//- hvis det ikke fungerer for dig, så vil jeg gøre det.
public class BotBehaviour : AntController
{
    public float randomMovementFactor = 0.25f;

    float timeCarryingRed = 0;

    //Angiv din bots navn her
    public override string GetPlayerName()
    {
        return "Antlion v1.0";
    }


    //Denne metode er main metoden du skal bruge for at få myren til at bevæge sig
    //deltatime er tiden siden sidste tick.
    //Hvis myrens antenner rammer et obstacle, så vil denne metode ikke blive kaldt dette tick.
    //Målet med denne metode er at returnere en retning som man ønsker myren skal bevæge sig hen imod. 
    //Myren vil ikke instantly ændre sin rotation til at passe med retningen, men vil accelerere sin rotation mod retningen.

    public override Vector2 ControlAnt(float deltaTime)
    {
        
        Vector2 desiredDirection = Vector2.one;
        float turnDegree = 50;
        
        if (HasAquiredATargetSugar() && !IsCarryingSugar())
        {
            SetTurnStrength(GetTurnStrength() * 2f);
            desiredDirection = GetDirectionToTargetSugar();

            if (AttemptToPickUpTargetSugar())
            {
                //do something smart here, perhaps
            }
        }
        else
        {
            //Eksempel på en måde at få myren til at bevæge sig på, så den går på en lidt tilfældig måde 
            randomMovementFactor += Random.Range(-1f, 1f) * deltaTime;
            if (Mathf.Abs(randomMovementFactor) > 0.45f)
            {
                randomMovementFactor = 0.15f;
            }
            else if (Mathf.Abs(randomMovementFactor) < 0.25f)
            {
                randomMovementFactor = 0.25f;
            }

            desiredDirection = GetOldDesiredDirection() + Random.insideUnitCircle * Mathf.Abs(randomMovementFactor);
        }

        AttemptToDepositSugar();

        PlacePheromone(PheromoneType.PLAYERCOLOR, 1, 65);
        
        //Eksempel på måde at få myren til at dreje kontrolleret - inkommenter og test (Jeg synes det her er en god måde at kontrollere direction)
        //desiredDirection = new Vector2(Mathf.Cos((GetCurrentRotation() - turnDegree) * Mathf.Deg2Rad), Mathf.Sin((GetCurrentRotation() - turnDegree) * Mathf.Deg2Rad)).normalized;


        /** kort hjælp til start
         * Sørg for at myrer udforsker mappet. 
         * Søg for at de placerer pheromones 
         * Brug pheromones til at få myrer til at signalere en path til sukker
         * for at finde hjem kunne man overveje noget om sandsynligheden for hvor koncentrationen af et pheromone er højt.
         * 
         */

        
        return desiredDirection;
    }
}
