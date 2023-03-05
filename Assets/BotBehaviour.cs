using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//RENAME GERNE DENNE KLASSE - Skal v�re forskelligt per spiller - Du skal ogs� �ndre klassens navn p� filen f�r det fungerer.
//derefter skal filen dragges over p� "Ant"-prefabben 
//- hvis det ikke fungerer for dig, s� vil jeg g�re det.
public class BotBehaviour : AntController
{
    public float randomMovementFactor = 0.25f;

    float timeCarryingRed = 0;

    //Angiv din bots navn her
    public override string GetPlayerName()
    {
        return "Antlion v1.0";
    }


    //Denne metode er main metoden du skal bruge for at f� myren til at bev�ge sig
    //deltatime er tiden siden sidste tick.
    //Hvis myrens antenner rammer et obstacle, s� vil denne metode ikke blive kaldt dette tick.
    //M�let med denne metode er at returnere en retning som man �nsker myren skal bev�ge sig hen imod. 
    //Myren vil ikke instantly �ndre sin rotation til at passe med retningen, men vil accelerere sin rotation mod retningen.

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
            //Eksempel p� en m�de at f� myren til at bev�ge sig p�, s� den g�r p� en lidt tilf�ldig m�de 
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
        
        //Eksempel p� m�de at f� myren til at dreje kontrolleret - inkommenter og test (Jeg synes det her er en god m�de at kontrollere direction)
        //desiredDirection = new Vector2(Mathf.Cos((GetCurrentRotation() - turnDegree) * Mathf.Deg2Rad), Mathf.Sin((GetCurrentRotation() - turnDegree) * Mathf.Deg2Rad)).normalized;


        /** kort hj�lp til start
         * S�rg for at myrer udforsker mappet. 
         * S�g for at de placerer pheromones 
         * Brug pheromones til at f� myrer til at signalere en path til sukker
         * for at finde hjem kunne man overveje noget om sandsynligheden for hvor koncentrationen af et pheromone er h�jt.
         * 
         */

        
        return desiredDirection;
    }
}
