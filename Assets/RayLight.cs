using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLight : MonoBehaviour
{
    public float distance;
    public Vector3 color;


   public void Draw(Vector3 positionA, Vector3 positionB)
   {   
      
   
      // r = distance + distance;
      // g = distance - distance;
      // b = r * g;

     float magnitude = (r + g + b);

     r = r / magnitude;
     g = g / magnitude;
     b = b / magnitude;

     Debug.DrawLine(positionA, positionB, new Colour(r,g,b,a)
   }
}
