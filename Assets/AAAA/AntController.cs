
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AntController : MonoBehaviour
{

    /** VIGTIGT! 
     
     
    En myre kan:
        - ændre hastighed
        - ændre kurs (rotation)
        - ændre hvor skarpt myren har tendens til at dreje 
        - se sukker hvis den er tæt på (se AntLogic for mere info)
        - se myretuen hvis den er tæt på (se AntLogic/PheromoneController for mere info)
        - lugte pheromones til højre og venstre (se AntLogic/PheromoneController for mere info) - Baser din movement på hvor pheromones er koncentreret
        - kan kun lugte 1 type pheromones af gangen (og der er cooldown på hvor tit myren kan lave en "smell"-reading)
        - vælge at placere enten rød eller blå pheromone (cooldown på 0.x sekunder (se antLogic))
        - bestemme styrken af pheromone, ved placering 

    En myre kender ikke
        - sin position noget tidspunkt. Det er derfor ikke lovligt at gemme et map af positioner og backtrack til anthill. 
            => det er ikke lovligt at udregne præcise positioner på nogen smarte måder, som fx ved at holde styr på tid, rotation og hastighed 
                * Det ER lovligt at holde styr på
                    hvor lang tid en myre bruger på at dreje til en side og bruge den information til at bryde et handlingsmønster (WINK WINK),
                    men det MÅ ALTSÅ IKKE BRUGES TIL AT UDREGNE ELLER GÆTTE positioner! 
        - positionen af andre myrer. (myrer kan ikke kommunikere med hinanden på andre måder end vha pheromones) Dvs fx statiske maps delt mellem myre er ulovlige.
            * Generelt så er det nemt at snyde, så lad være med at finde på noget smart - (Fair play). 
              Jeg tjekker koden - Og meget nøje hvis du vinder - 99% af interesse; fordi jeg synes det er spændende hvad du finder på :)


    Pheromones
        - fordamper over tid 
        - der er 2 slags; rød og blå. Du bestemmer hvad de skal bruges til. 
        - røde pheromones har kortere max-lifetime
        
   

     Lovlige kald:
        - Alle public metoder i denne klasse. 


    Ikke lovlige kode:
        - kald som henter informationer om gameObject eller transform etc. 
            *(fx this.gameObject, targetSugar.gameobject, this.transform.position eller this.gameObject.getComponent<SomeClass>))
        
  

    Encouragement.
        Jeg lover opgaven kan løses med de regler der er opsat - det er verificeret af moi.

     */
    private AntLogic antLogic;
    private SugarLogic targetSugar;
    private float bluePheromoneStrengthLeft;
    private float bluePheromoneStrengthRight;
    private float redPheromoneStrengthLeft;
    private float redPheromoneStrengthRight;

    public float GetBluePheromoneStrengthLeft() { return bluePheromoneStrengthLeft; }
    public float GetBluePheromoneStrengthRight() { return bluePheromoneStrengthRight; }
    public float GetRedPheromoneStrengthLeft() { return redPheromoneStrengthLeft; }
    public float GetRedPheromoneStrengthRight() { return redPheromoneStrengthRight; }



    /*
        public float randomMovementFactor = 0.4f;

        AntLogic antLogic;

      
        protected SugarLogic targetSugar;
        float timeCarryingRed = 0;
        public string playerName { get; } = "Antlion69 - sæt til dit nickname"; 
    */
    private void Awake()
    {
        antLogic = GetComponent<AntLogic>();
    }



    public abstract string GetPlayerName();

    //Denne metode er main metoden du skal bruge for at få myren til at bevæge sig
    //deltatime er tiden siden sidste tick.
    //Hvis myrens antenner rammer et obstacle, så vil denne metode ikke blive kaldt dette tick.
    //Målet med denne metode er at returnere en retning som man ønsker myren skal bevæge sig hen imod. 
    //Myren vil ikke instantly ændre sin rotation til at passe med retningen, men vil accelerere sin rotation mod retningen.
    public abstract Vector2 ControlAnt(float deltaTime);

    public Vector2 GetOldDesiredDirection()
    {
        return antLogic.getDesiredDirection();
    }

    //Forsøger at aflevere sukker i myreboet
    public bool AttemptToDepositSugar()
    {
        bool depositedSugar = antLogic.attemptToDepositSugar(targetSugar);
        if (depositedSugar) {
            targetSugar = null;
        }
        return depositedSugar;
    }

    
    public bool CanDetectRedPheromone()
    {
        SensePheromones(PheromoneType.RED);

        return redPheromoneStrengthLeft + redPheromoneStrengthRight > 0.1f;
    }

    //der er cooldown på hvor tit pheromones kan placeres.
    //røde pheromones kan leve op til 25 sekunder
    //blå pheromones kan leve op til 65 sekunder
    //initialPheromoneStrength er minimum 1 og maximalt 10
    public void PlacePheromone(PheromoneType type, float initialPheromoneStrength = 1, float lifetime = 65f) {
        antLogic.placePheromone(type, initialPheromoneStrength, lifetime);
    }


    public float GetCurrentTime() {
        return Time.time;
    }


    /** Du kan kun spore én lugt af gangen, og der er et lille cooldown på hvor ofte myren kan sense nye pheromones (performance optimization)*/
    public void SensePheromones(PheromoneType pheromoneType)
    {
        antLogic.SensePheromones(pheromoneType);
        bluePheromoneStrengthLeft = antLogic.getBluePheromoneStrengthLeft();
        bluePheromoneStrengthRight = antLogic.getBluePheromoneStrengthRight();
        redPheromoneStrengthLeft = antLogic.getRedPheromoneStrengthLeft();
        redPheromoneStrengthRight = antLogic.getRedPheromoneStrengthRight();

    }

    //Når myren kommer tæt på noget sukker vil denne metode blive kørt, og derefter er det muligt at finde retningen hen til sukkeret vha metoden nedenfor
    public void FoodLocated(SugarLogic sugarLogic)
    {
        if (targetSugar == null && sugarLogic.aquireAsTarget())
        {
            targetSugar = sugarLogic;
        }
    }

    //returnerer retningen til sukker, såfremt myren har sporet et target sukker
    public Vector2 GetDirectionToTargetSugar()
    {
        if (targetSugar != null)
        {
            return targetSugar.getDirectionToSugar(this);
        }
        return Vector2.zero;
    }

    //Returnerer true, hvis myren har fundet et stykke sukker; (Dette er ikke lig med at sukkeret er samlet op!)
    public bool HasAquiredATargetSugar() {
        return targetSugar != null;
    }

    //Forsøg at saml sukker op
    public bool AttemptToPickUpTargetSugar() {
        if (targetSugar == null) {
            return false;
        }
        return antLogic.attemptToPickUpSugar(targetSugar);
    }

    //Her kan du få myren til at gå langsommere eller helt stoppe den. 
    public void SlowAntDownByFactor(float v)
    {
        antLogic.slowAntByFactor(v);
    }

    //Her kan hastigheden af hvor hurtigt myren kan ændre retning justeres (Max er 2.5)
    public void SetTurnStrength(float turnStrength) {
        antLogic.setTurnStrength(turnStrength);
    }

    //Get på turnspeed hastigheden
    public float GetTurnStrength() {
        return antLogic.getTurnStrength();
    }

    //Finder myrens rotation
    public float GetCurrentRotation() {
        return antLogic.getCurrentRotation();
    }

    //Hvis Myren lige nu slæber rundt med et stykke sukker, returneres true
    public bool IsCarryingSugar() {
        return antLogic.isCarryingSugar();
    }

    //Hvis myren er tæt på sin myreture vil denne metode give retningen derhen
    public Vector2 GetDirectionToAnthill() {
        return antLogic.getDirectionToAnthill();
    }

}
