using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public SpriteRenderer rend;

    public Sprite activePoint;
    public Sprite inactivePoint;

    public void ChangeActiveState(bool state) 
    {
        if (state)
            rend.sprite = activePoint;
        else
            rend.sprite = inactivePoint;
    }
}
