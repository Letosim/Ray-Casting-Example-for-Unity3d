using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLight : MonoBehaviour
{
    public float distance;
    public Vector3 color;


   public void Draw()
   {   
      
   
      // r = distance + distance;
      // g = distance - distance;
      // b = r * g;

     float magnitude = (r + g + b);

     r = r / magnitude;
     g = g / magnitude;
     b = b / magnitude;

    
   }
}
