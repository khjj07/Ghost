using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Rotate()
    {
       
        var tmp = GetComponent<Mirror>().Normal;
        GetComponent<Mirror>().Normal.x = GetComponent<Mirror>().Normal.y;
        GetComponent<Mirror>().Normal.y = -tmp.x;
        GetComponent<Mirror>().EnableVector1 = GetComponent<Mirror>().Normal.x > 0 ? Vector2.left : Vector2.right ;
        GetComponent<Mirror>().EnableVector2 = GetComponent<Mirror>().Normal.y > 0 ? Vector2.down : Vector2.up;
        spriteRenderer.flipX = GetComponent<Mirror>().Normal.x>0 ? false : true;
         spriteRenderer.flipY = GetComponent<Mirror>().Normal.y>0 ? true : false;
      
    }
}
