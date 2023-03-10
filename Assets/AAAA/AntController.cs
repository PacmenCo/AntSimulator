
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AntController : MonoBehaviour
{

    /** VIGTIGT! 
     
     
    En myre kan:
        - �ndre hastighed
        - �ndre kurs (rotation)
        - �ndre hvor skarpt myren har tendens til at dreje 
        - se sukker hvis den er t�t p� (se AntLogic for mere info)
        - se myretuen hvis den er t�t p� (se AntLogic/PheromoneController for mere info)
        - lugte pheromones til h�jre og venstre (se AntLogic/PheromoneController for mere info) - Baser din movement p� hvor pheromones er koncentreret
        - kan kun lugte 1 type pheromones af gangen (og der er cooldown p� hvor tit myren kan lave en "smell"-reading)
        - v�lge at placere enten r�d eller bl� pheromone (cooldown p� 0.x sekunder (se antLogic))
        - bestemme styrken af pheromone, ved placering 

    En myre kender ikke
        - sin position noget tidspunkt. Det er derfor ikke lovligt at gemme et map af positioner og backtrack til anthill. 
            => det er ikke lovligt at udregne pr�cise positioner p� nogen smarte m�der, som fx ved at holde styr p� tid, rotation og hastighed 
                * Det ER lovligt at holde styr p�
                    hvor lang tid en myre bruger p� at dreje til en side og bruge den information til at bryde et handlingsm�nster (WINK WINK),
                    men det M� ALTS� IKKE BRUGES TIL AT UDREGNE ELLER G�TTE positioner! 
        - positionen af andre myrer. (myrer kan ikke kommunikere med hinanden p� andre m�der end vha pheromones) Dvs fx statiske maps delt mellem myre er ulovlige.
            * Generelt s� er det nemt at snyde, s� lad v�re med at finde p� noget smart - (Fair play). 
              Jeg tjekker koden - Og meget n�je hvis du vinder - 99% af interesse; fordi jeg synes det er sp�ndende hvad du finder p� :)


    Pheromones
        - fordamper over tid 
        - der er 2 slags; r�d og bl�. Du bestemmer hvad de skal bruges til. 
        - r�de pheromones har kortere max-lifetime
        
   

     Lovlige kald:
        - Alle public metoder i denne klasse. 


    Ikke lovlige kode:
        - kald som henter informationer om gameObject eller transform etc. 
            *(fx this.gameObject, targetSugar.gameobject, this.transform.position eller this.gameObject.getComponent<SomeClass>))
        
  

    Encouragement.
        Jeg lover opgaven kan l�ses med de regler der er opsat - det er verificeret af moi.

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
        public string playerName { get; } = "Antlion69 - s�t til dit nickname"; 
    */
    private void Awake()
    {
        antLogic = GetComponent<AntLogic>();
    }



    public abstract string GetPlayerName();

    //Denne metode er main metoden du skal bruge for at f� myren til at bev�ge sig
    //deltatime er tiden siden sidste tick.
    //Hvis myrens antenner rammer et obstacle, s� vil denne metode ikke blive kaldt dette tick.
    //M�let med denne metode er at returnere en retning som man �nsker myren skal bev�ge sig hen imod. 
    //Myren vil ikke instantly �ndre sin rotation til at passe med retningen, men vil accelerere sin rotation mod retningen.
    public abstract Vector2 ControlAnt(float deltaTime);

    public Vector2 GetOldDesiredDirection()
    {
        return antLogic.getDesiredDirection();
    }

    //Fors�ger at aflevere sukker i myreboet
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

    //der er cooldown p� hvor tit pheromones kan placeres.
    //r�de pheromones kan leve op til 25 sekunder
    //bl� pheromones kan leve op til 65 sekunder
    //initialPheromoneStrength er minimum 1 og maximalt 10
    public void PlacePheromone(PheromoneType type, float initialPheromoneStrength = 1, float lifetime = 65f) {
        antLogic.placePheromone(type, initialPheromoneStrength, lifetime);
    }


    public float GetCurrentTime() {
        return Time.time;
    }


    /** Du kan kun spore �n lugt af gangen, og der er et lille cooldown p� hvor ofte myren kan sense nye pheromones (performance optimization)*/
    public void SensePheromones(PheromoneType pheromoneType)
    {
        antLogic.SensePheromones(pheromoneType);
        bluePheromoneStrengthLeft = antLogic.getBluePheromoneStrengthLeft();
        bluePheromoneStrengthRight = antLogic.getBluePheromoneStrengthRight();
        redPheromoneStrengthLeft = antLogic.getRedPheromoneStrengthLeft();
        redPheromoneStrengthRight = antLogic.getRedPheromoneStrengthRight();

    }

    //N�r myren kommer t�t p� noget sukker vil denne metode blive k�rt, og derefter er det muligt at finde retningen hen til sukkeret vha metoden nedenfor
    public void FoodLocated(SugarLogic sugarLogic)
    {
        if (targetSugar == null && sugarLogic.aquireAsTarget())
        {
            targetSugar = sugarLogic;
        }
    }

    //returnerer retningen til sukker, s�fremt myren har sporet et target sukker
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

    //Fors�g at saml sukker op
    public bool AttemptToPickUpTargetSugar() {
        if (targetSugar == null) {
            return false;
        }
        return antLogic.attemptToPickUpSugar(targetSugar);
    }

    //Her kan du f� myren til at g� langsommere eller helt stoppe den. 
    public void SlowAntDownByFactor(float v)
    {
        antLogic.slowAntByFactor(v);
    }

    //Her kan hastigheden af hvor hurtigt myren kan �ndre retning justeres (Max er 2.5)
    public void SetTurnStrength(float turnStrength) {
        antLogic.setTurnStrength(turnStrength);
    }

    //Get p� turnspeed hastigheden
    public float GetTurnStrength() {
        return antLogic.getTurnStrength();
    }

    //Finder myrens rotation
    public float GetCurrentRotation() {
        return antLogic.getCurrentRotation();
    }

    //Hvis Myren lige nu sl�ber rundt med et stykke sukker, returneres true
    public bool IsCarryingSugar() {
        return antLogic.isCarryingSugar();
    }

    //Hvis myren er t�t p� sin myreture vil denne metode give retningen derhen
    public Vector2 GetDirectionToAnthill() {
        return antLogic.getDirectionToAnthill();
    }

}
