using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLight : MonoBehaviour
{
    [Header("Light Properties")]
    public float distance; // The maximum range of the light
    public Vector3 color;  // The RGB color of the light, represented as a vector

    /*
     * This class represents a light source in the scene.
     * - `distance`: Defines how far the light can reach.
     * - `color`: Specifies the color of the light in RGB, with each component between 0 and 1.
     * The light's position is taken from the Transform component of the GameObject it is attached to.
     */
}