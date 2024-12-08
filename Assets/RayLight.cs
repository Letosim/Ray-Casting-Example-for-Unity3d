using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLight : MonoBehaviour
{
    public float distance;
    public Vector3 color;


   public void Draw(Vector3 positionA, Vector3 positionB , int depth, int maxDepth)
   {   
       r = distance + distance;
       g = r / distance;
       b = r + g;

     float magnitude = (r + g + b);

     r = r / magnitude;
     g = g / magnitude;
     b = b / magnitude;

     a = (float) depth / (float) maxDepth;

     Debug.DrawLine(positionA, positionB, new Color(r,g,b,a);
   }
   

public void Draw(Vector3 positionA, Vector3 positionB)
{
   Debug.DrawLine(positionA, positionB, color);
}


public void Draw(Vector3 positionA, Vector3 positionB, Vector3 color)
   {
 Debug.DrawLine(positionA, positionB, color * (f1-((float) depth / (float) maxDepth))  );

   }
}
