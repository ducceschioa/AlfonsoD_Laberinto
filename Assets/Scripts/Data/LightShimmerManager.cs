using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//ESTE SCRIPT HACE QUE LAS LUCES EN OBJETOS HIJOS PARPADEEN.
public class LightShimmerManager : MonoBehaviour
{
    List<Light2D> lights;
    List<float> speeds;

    float speedRegulator = 0.3f;

    public bool lighterEffect = false;

    public bool increasing;
    //float target;

    void Awake()
    {
        lights = new List<Light2D>();
        speeds = new List<float>();

        for (int i = 0; i < transform.childCount; i++) {
            lights.Add(transform.GetChild(i).GetComponent<Light2D>());
            speeds.Add(transform.GetChild(i).GetComponent<Light2D>().intensity);
        }
    }

    private void Update()
    {
        for (int i = 0; i < lights.Count; i++) {

            if (lighterEffect == false)
            {
                if (increasing == true)
                {
                    lights[i].intensity += Time.deltaTime * speeds[i] * speedRegulator;
                    if (lights[i].intensity > speeds[i] * 2f)
                    {
                        increasing = false;
                    }
                }
                else
                {
                    lights[i].intensity -= Time.deltaTime * speeds[i] * speedRegulator;
                    if (lights[i].intensity < speeds[i] / 2f)
                    {
                        increasing = true;
                    }
                }
            }
            else
            {
                if (increasing == true)
                {
                    lights[i].intensity += Time.deltaTime * speeds[i] * speedRegulator;
                    if (lights[i].intensity > speeds[i] * 1.2f)
                    {
                        increasing = false;
                    }
                }
                else
                {
                    lights[i].intensity -= Time.deltaTime * speeds[i] * speedRegulator;
                    if (lights[i].intensity < speeds[i] / 1.2f)
                    {
                        increasing = true;
                    }
                }

            }
        }
    }
}
