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

        for (int i = 0; i < steps; i++)
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


            if (hitPointCamera.collider != null)
            {
                if (i == 0 || i == steps - 1)
                   //Debug.DrawLine(Camera.main.ScreenToWorldPoint(n//ew Vector3(x, y, 0)), hitPointCamera.point, //Color.red);

                Vector3 albedo = Vector3.zero;

                albedo += WorldPointToLightRay(hitPointCamera.point + hitPointCamera.normal * .01f, i);



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

                            //reflectionPoint
                            Debug.Log("?");
                            //Debug.DrawLine(reflectionPoint, //hitPointCamera.point, Color.magenta);


Draw(reflectionPoint, hitPointCamera.point, hitPointCamera.point, albedo);

                            Physics.Raycast(hitPointCamera.point, direction, out hitPointCamera, 5000);

                            reflectionPoint = hitPointCamera.point;

                            albedoReflections += WorldPointToLightRay(hitPointCamera.point + hitPointCamera.normal * .01f, i);
                            reflections++;
                        }
                    }

                    if (reflectionPases != 0)
                    {
                        albedoReflections /= reflections;
                        albedo = (albedo + albedoReflections) / 2f;




                    }
                }

                texture.SetPixel(x, y, new Color(albedo.x, albedo.y, albedo.z));

            }

            x++;
            if (x == width)
            {
                x = 0;
                y++;
            }


        }
        return;
    }

    public Vector3 WorldPointToLightRay(Vector3 origin, int stepIndex)
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
                    if (stepIndex == 0 || stepIndex == steps - 1)
                        Debug.DrawLine(origin, lights[l].transform.position, Color.white);

                    float delta = 1f - (distance / lights[l].distance);

                    albedo.x += delta * lights[l].color.x;
                    albedo.y += delta * lights[l].color.y;
                    albedo.z += delta * lights[l].color.z;
                }
            }
        }

        return albedo;
    }

    public Vector3 WorldPointToLightRay(Vector3 origin)
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
                    float delta = 1f - (distance / lights[l].distance);

                    albedo.x += delta * lights[l].color.x;
                    albedo.y += delta * lights[l].color.y;
                    albedo.z += delta * lights[l].color.z;
                }
            }
        }

        return albedo;
    }


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


public void Draw(Vector3 positionA, Vector3 positionB, Color color)
{
   Debug.DrawLine(positionA, positionB, color);
}


public void Draw(Vector3 positionA, Vector3 positionB, Vector3 color)
   {
 Debug.DrawLine(positionA, positionB, color * (f1-((float) depth / (float) maxDepth))  );

   }


}
