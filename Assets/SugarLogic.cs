using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarLogic : MonoBehaviour
{

    bool targetted = false;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        StartCoroutine(disableCollidersAndOptimizePerformance());
    }

    IEnumerator disableCollidersAndOptimizePerformance()
    {
        yield return new WaitForSeconds(8);
        GetComponent<CircleCollider2D>().isTrigger = true;
    }


    public void pickUp() {
        GetComponent<CircleCollider2D>().enabled = false;
        Destroy(GetComponent<Rigidbody2D>());
    }
    
    internal bool aquireAsTarget()
    {
        if (targetted)
            return false;
        
        targetted = true;
        return true;
    }

    internal bool isTargetted()
    {
        return targetted;
    }

    internal void PickUpFood() { 
    
    }


    internal Vector2 getDirectionToSugar(AntController antController)
    {
        return (this.gameObject.transform.position - antController.transform.position).normalized;
    }
}
