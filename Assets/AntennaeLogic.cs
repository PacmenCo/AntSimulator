using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaeLogic : MonoBehaviour
{
    public bool isColliding = false;
    SpriteRenderer sprite;
    AntLogic antlogic;

    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        antlogic = GetComponentInParent<AntLogic>();
    }


    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            isColliding = true;
            sprite.color = Color.red;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            isColliding = false;
            sprite.color = Color.black;
        }
    }
}
