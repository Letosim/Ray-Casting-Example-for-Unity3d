using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    [Header("Light and Material Settings")]
    public RayLight[] lights; // Array of light sources in the scene
    public Material material; // Material to display the rendered texture

    [Header("Raycasting Settings")]
    public int reflectionPasses = 10; // Number of reflective ray bounces
    public int steps = 1; // Number of pixels processed per frame

    private Texture2D texture; // Texture for storing pixel data
    private int width; // Screen width in pixels
    private int height; // Screen height in pixels

    private int currentX = 0; // Current X-coordinate being processed
    private int currentY = 0; // Current Y-coordinate being processed
    private bool isRenderingComplete = false; // Flag to indicate if rendering is finished

    void Start()
    {
        // Initialize texture dimensions based on screen size
        width = (int)Camera.main.pixelRect.width;
        height = (int)Camera.main.pixelRect.height;

        // Create a new texture and assign it to the material
        texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        material.mainTexture = texture;

        Debug.Log($"Render Start Time: {System.DateTime.Now.Millisecond} ms");
    }

    void Update()
    {
        // If rendering is complete, do nothing
        if (isRenderingComplete) return;

        // Apply the texture to the material when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            texture.Apply();
        }

        // Process the specified number of pixels per frame
        for (int i = 0; i < steps; i++)
        {
            // Check if all pixels are processed
            if (currentY >= height)
            {
                isRenderingComplete = true;
                texture.Apply(); // Apply the final texture
                Debug.Log($"Render Complete Time: {System.DateTime.Now.Millisecond} ms");
                return;
            }

            // Process the current pixel
            ProcessPixel(currentX, currentY);

            // Move to the next pixel
            currentX++;
            if (currentX >= width)
            {
                currentX = 0; // Reset X-coordinate
                currentY++; // Move to the next row
            }
        }
    }

    /// <summary>
    /// Processes a single pixel by casting rays and calculating lighting.
    /// </summary>
    private void ProcessPixel(int x, int y)
    {
        // Cast a ray from the camera through the current pixel
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
        if (Physics.Raycast(ray, out RaycastHit hitPointCamera, 5000))
        {
            // Calculate the direct lighting at the hit point
            Vector3 albedo = CalculateDirectLighting(hitPointCamera.point + hitPointCamera.normal * 0.01f);

            // If the surface is reflective, calculate reflections
            if (hitPointCamera.collider.GetComponent<IsReflective>())
            {
                albedo = CalculateReflections(hitPointCamera, albedo);
            }

            // Set the calculated color to the texture
            texture.SetPixel(x, y, new Color(albedo.x, albedo.y, albedo.z));
        }
    }

    /// <summary>
    /// Calculates direct lighting contributions at a given point.
    /// </summary>
    private Vector3 CalculateDirectLighting(Vector3 origin)
    {
        Vector3 totalLight = Vector3.zero;

        // Iterate over all light sources
        foreach (var light in lights)
        {
            float distance = Vector3.Distance(origin, light.transform.position);

            // Check if the point is within the light's range and not occluded
            if (distance < light.distance && !Physics.Raycast(origin, light.transform.position - origin, out _, distance))
            {
                float intensityFactor = 1f - (distance / light.distance); // Calculate light attenuation
                totalLight += intensityFactor * light.color; // Accumulate light contribution
            }
        }

        return totalLight;
    }

    /// <summary>
    /// Calculates lighting contributions from reflections.
    /// </summary>
    private Vector3 CalculateReflections(RaycastHit initialHit, Vector3 initialAlbedo)
    {
        Vector3 accumulatedReflections = Vector3.zero;
        Vector3 currentDirection = Vector3.Reflect(Camera.main.transform.forward, initialHit.normal);
        Vector3 currentPoint = initialHit.point;

        // Perform raycasting for the specified number of reflection passes
        for (int i = 0; i < reflectionPasses; i++)
        {
            // Cast a ray for reflection
            if (!Physics.Raycast(currentPoint, currentDirection, out RaycastHit reflectionHit, 5000)) break;

            // Visualize the reflection ray
            Debug.DrawLine(currentPoint, reflectionHit.point, Color.magenta);

            // Update direction and point for the next reflection
            currentDirection = Vector3.Reflect(currentDirection, reflectionHit.normal);
            currentPoint = reflectionHit.point + reflectionHit.normal * 0.01f;

            // Add the light contribution from the reflection
            accumulatedReflections += CalculateDirectLighting(currentPoint);

            // Stop if the surface is not reflective
            if (reflectionHit.collider == null || !reflectionHit.collider.GetComponent<IsReflective>())
            {
                break;
            }
        }

        // Blend the initial lighting with the reflections
        return reflectionPasses > 0 ? (initialAlbedo + accumulatedReflections / reflectionPasses) / 2f : initialAlbedo;
    }
}