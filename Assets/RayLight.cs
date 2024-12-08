using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLight : MonoBehaviour
{
    public float distance;
    public Vector3 color;


   public void Draw(Vector3 positionA, Vector3 positionB , int depth, int maxDepth)
   {   
      
   
      // r = distance + distance;
      // g = distance - distance;
      // b = r * g;

     float magnitude = (r + g + b);

     r = r / magnitude;
     g = g / magnitude;
     b = b / magnitude;
     a = f1-((float) depth / (float) maxDepth);
     Debug.DrawLine(positionA, positionB, new Color(r,g,b,a);
   }
   

public void Draw(Vector3 positionA, Vector3 positionB, Vector3 color)
{
 Debug.DrawLine(positionA, positionB, color *   .1f);
}


public void Draw(Vector3 positionA, Vector3 positionB, Vector3 color)
{

a = (f1-((float) depth / (float) maxDepth));
     Debug.DrawLine(positionA, positionB, new Color(r,g,b,a);

 Debug.DrawLine(positionA, positionB, color * (f1-((float) depth / (float) maxDepth))  );
}



//light



}
