using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public RayLight[] lights;

    public Material material;

    private Texture2D texture;
    private int width;
    private int height;

    public int reflectionPases = 10;
    public int steps = 1;

    public bool useFiveRays = true;


    // Start is called before the first frame update
    void Start()
    {
        width = (int)Camera.main.pixelRect.width;
        height = (int)Camera.main.pixelRect.height;
        texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        material.mainTexture = texture;
        Debug.Log(System.DateTime.Now.Millisecond);
    }

    private int x = 0;
    private int y = 0;
    bool done = false;

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            texture.Apply();

        int rc = 0;

        for (int i = 0; i < steps; i++)
        {
            Vector3 albedo = Vector3.zero;

            if (useFiveRays == true)
                rc = 5;
            else
                rc = 0;

            for (int o = 0; o < rc; o++)
            {
                if (y == height)
                {
                    done = true;
                    texture.Apply();
                    Debug.Log(System.DateTime.Now.Millisecond);
                    return;
                }

                RaycastHit hitPointCamera;

                Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(x, y, 0)), out hitPointCamera, 5000);

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                GenerateSpline(hitPointCamera.point, Vector3.Cross(hitPointCamera.normal, Vector3.up).normalized);


GenerateSpline(hitPointCamera.point, Vector3.Cross(hitPointCamera.normal, Vector3.right).normalized);


GenerateSpline(hitPointCamera.point, Vector3.Cross(hitPointCamera.normal, Vector3.bottum).normalized);


GenerateSpline(hitPointCamera.point, Vector3.Cross(hitPointCamera.normal, Vector3.left).normalized);


                if (o == 0) Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(x, y, 0)), out hitPointCamera, 5000);
                else
                {
                    Vector3 origin = Camera.main.ScreenPointToRay(new Vector3(x + 15.1f, y, 0)).origin;
                    Vector3 dir = Camera.main.ScreenPointToRay(new Vector3(x + 15.1f, y, 0)).direction;


                    if (o == 1)
                        Physics.Raycast(origin, dir, out hitPointCamera, 5000);

                    origin = Camera.main.ScreenPointToRay(new Vector3(x + 15.1f, y, 0)).origin;
                    dir = Camera.main.ScreenPointToRay(new Vector3(x + 15.1f, y, 0)).direction;

                    if (o == 2)
                        Physics.Raycast(origin, dir, out hitPointCamera, 5000);

                    origin = Camera.main.ScreenPointToRay(new Vector3(x, y + 15.1f, 0)).origin;
                    dir = Camera.main.ScreenPointToRay(new Vector3(x, y + 15.1f, 0)).direction;


                    if (o == 3)
                        Physics.Raycast(origin, dir, out hitPointCamera, 5000);

                    origin = Camera.main.ScreenPointToRay(new Vector3(x, y - 15.1f, 0)).origin;
                    dir = Camera.main.ScreenPointToRay(new Vector3(x, y - 15.1f, 0)).direction;

                    if (o == 4)
                        Physics.Raycast(origin, dir, out hitPointCamera, 5000);



                    //if (o == 1)
                    //    Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(x + 1, y, 0)), out hitPointCamera, 5000);
                    //if (o == 2)
                    //    Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(x - 1, y, 0)), out hitPointCamera, 5000);
                    //if (o == 3)
                    //    Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(x, y + 1, 0)), out hitPointCamera, 5000);
                    //if (o == 4)
                    //    Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(x, y - 1, 0)), out hitPointCamera, 5000);
                }

                if (hitPointCamera.collider != null)
                {
                    albedo += WorldPointToLightRay(hitPointCamera.point + hitPointCamera.normal * .01f, hitPointCamera.collider.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D);

                    if (i == 0 || i == steps - 1)
                        Draw(Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0)), hitPointCamera.point, albedo);

                    Vector3 albedoReflections = Vector3.zero;

                    if (hitPointCamera.collider.transform.GetComponent<IsReflective>())
                    {
                        float reflections = 0;

                        Vector3 direction = Vector3.Reflect(Camera.main.transform.forward, hitPointCamera.normal);

                        Vector3 reflectionPoint = hitPointCamera.point;

                        for (int r = 0; r < reflectionPases; r++)
                        {
                            if (r != 0)
                                direction = Vector3.Reflect(direction, hitPointCamera.normal);

                            if (hitPointCamera.collider != null && (r == 0 || hitPointCamera.collider.transform.GetComponent<IsReflective>()))
                            {
                                Physics.Raycast(hitPointCamera.point, direction, out hitPointCamera, 5000);

                                if (hitPointCamera.collider != null)
                                    albedoReflections += WorldPointToLightRay(hitPointCamera.point + hitPointCamera.normal * .01f, reflectionPoint, r, hitPointCamera.collider.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D);

                                reflectionPoint = hitPointCamera.point;

                                reflections++;
                            }
                        }

                        if (reflectionPases != 0)
                        {
                            albedoReflections /= reflections;
                            albedo = (albedo + albedoReflections) / 2f;
                        }
                    }
                }

                //albedo /= (float)rc; ab (if distance < 2f)     1f - !?  color  * distance / 2f

                texture.SetPixel(x, y, new Color(albedo.x, albedo.y, albedo.z));
            }

            albedo /= (float)rc;


            x++;
            if (x == width)
            {
                x = 0;
                y++;
            }


        }

        return;
    }

    public Vector3 WorldPointToLightRay(Vector3 origin, int stepIndex, Texture2D texture)
    {
        Vector3 albedo = Vector3.zero;

        for (int l = 0; l < lights.Length; l++)
        {
            RaycastHit hitPointLight;
            float distance = Vector3.Distance(origin, lights[l].transform.position);

            if (distance < lights[l].distance)
            {
                Physics.Raycast(origin, lights[l].transform.position - origin, out hitPointLight, distance);

                if (hitPointLight.collider != null && (stepIndex == 0 || stepIndex == steps - 1))
                    Debug.DrawLine(origin, hitPointLight.point, Color.cyan);

                if (hitPointLight.collider == null)
                {
                    Color textureColor = texture.GetPixel((int)(hitPointLight.textureCoord.x * texture.width), (int)(hitPointLight.textureCoord.y * texture.height));

                    float delta = 1f - (distance / lights[l].distance);

                    albedo.x += delta * (lights[l].color.x + textureColor.r) / 2f;
                    albedo.y += delta * (lights[l].color.y + textureColor.g) / 2f;
                    albedo.z += delta * (lights[l].color.z + textureColor.b) / 2f;

                }
            }


        }

        return albedo;
    }

    public Vector3 WorldPointToLightRay(Vector3 origin, Vector3 targetDrawVector, int stepIndex, Texture2D texture)
    {
        Vector3 albedo = Vector3.zero;

        for (int l = 0; l < lights.Length; l++)
        {
            RaycastHit hitPointLight;
            float distance = Vector3.Distance(origin, lights[l].transform.position);

            if (distance < lights[l].distance)
            {
                Physics.Raycast(origin, lights[l].transform.position - origin, out hitPointLight, distance);

                if (hitPointLight.collider == null)
                {
                    Color textureColor = texture.GetPixel((int)(hitPointLight.textureCoord.x * texture.width), (int)(hitPointLight.textureCoord.y * texture.height));

                    float delta = 1f - (distance / lights[l].distance);

                    albedo.x += delta * (lights[l].color.x + textureColor.r) / 2f;
                    albedo.y += delta * (lights[l].color.y + textureColor.g) / 2f;
                    albedo.z += delta * (lights[l].color.z + textureColor.b) / 2f;

                    Draw(origin, targetDrawVector, albedo, 1f + delta);
                }
            }
        }

        return albedo;
    }

    public Vector3 WorldPointToLightRay(Vector3 origin, Texture2D texture)
    {
        Vector3 albedo = Vector3.zero;

        for (int l = 0; l < lights.Length; l++)
        {
            RaycastHit hitPointLight;
            float distance = Vector3.Distance(origin, lights[l].transform.position);

            if (distance < lights[l].distance)
            {

                Physics.Raycast(origin, lights[l].transform.position - origin, out hitPointLight, distance);

                if (hitPointLight.collider == null)
                {
                    Color textureColor = texture.GetPixel((int)(hitPointLight.textureCoord.x * texture.width), (int)(hitPointLight.textureCoord.y * texture.height));

                    float delta = 1f - (distance / lights[l].distance);

                    albedo.x += delta * (lights[l].color.x + textureColor.r) / 2f;
                    albedo.y += delta * (lights[l].color.y + textureColor.g) / 2f;
                    albedo.z += delta * (lights[l].color.z + textureColor.b) / 2f;

                    Draw(origin, lights[l].transform.position, albedo);
                }
            }
        }

        return albedo;
    }


    public void Draw(Vector3 positionA, Vector3 positionB, int depth, int maxDepth)
    {
        float delta = (float)depth / (float)maxDepth;

        float r = delta + delta;
        float g = r / delta;
        float b = r + g;

        float magnitude = (r + g + b);

        r = r / magnitude;
        g = g / magnitude;
        b = b / magnitude;

        float a = (float)depth / (float)maxDepth;

        Debug.DrawLine(positionA, positionB, new Color(r, g, b, a));
    }


    public void Draw(Vector3 positionA, Vector3 positionB, Color color)
    {
        Debug.DrawLine(positionA, positionB, color);
    }


    public void Draw(Vector3 positionA, Vector3 positionB, Vector3 color)
    {
        Debug.DrawLine(positionA, positionB, new Color(color.x, color.y, color.z));// * (1f  - ((float)depth / (float)maxDepth)));

    }

    public void Draw(Vector3 positionA, Vector3 positionB, Vector3 color, float alpha)
    {
        Debug.DrawLine(positionA, positionB, new Color(color.x, color.y, color.z, alpha));// * (1f  - ((float)depth / (float)maxDepth)));

    }

    //################################################################################################################################## ChatGPT
    public LayerMask hitLayer;
    public int splineResolution = 20; // Number of points on the spline
    public float edgeLength = 10f;    // Length to follow the edge

    private List<Vector3> controlPoints = new List<Vector3>();



    void GenerateSpline(Vector3 hitPoint, Vector3 direction)
    {
        //normal = normal * .1f;

        controlPoints.Clear();
        controlPoints.Add(hitPoint); // Add the initial hit point

      Vector3 currentPoint = hitPoint;//+normal >_>


        






        

        for (int i = 1; i <= 4; i++) // Generate 4 additional control points along the edge
        {
            currentPoint += direction * (edgeLength / 4); // Move along the edge


//cast ray hitpointDir



            controlPoints.Add(currentPoint);
        }

        DrawSpline();
    }

    void DrawSpline()
    {
        List<Vector3> splinePoints = GenerateCatmullRomSpline(controlPoints, splineResolution);

        //lineRenderer.positionCount = splinePoints.Count;
        //lineRenderer.SetPositions(splinePoints.ToArray());
        Debug.DrawLine(splinePoints[0], splinePoints[1], Color.green);

        for (int l = 1; l < splinePoints.Count; l++)
            Debug.DrawLine(splinePoints[l], splinePoints[l - 1], Color.green);

    }

    List<Vector3> GenerateCatmullRomSpline(List<Vector3> points, int resolution)
    {
        List<Vector3> spline = new List<Vector3>();
        if (points.Count < 4)
        {
            Debug.LogError("Need at least 4 control points for Catmull-Rom spline.");
            return spline;
        }

        for (int i = 0; i < points.Count - 3; i++)
        {
            for (int j = 0; j <= resolution; j++)
            {
                float t = j / (float)resolution;
                spline.Add(CatmullRom(points[i], points[i + 1], points[i + 2], points[i + 3], t));
            }
        }

        return spline;
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * ((2f * p1) +
                       (-p0 + p2) * t +
                       (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                       (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }
}
