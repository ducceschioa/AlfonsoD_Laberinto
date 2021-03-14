using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT QUE HACE QUE LA LINTERNA SE APARTE CUANDO SE ACERCA A UN MURO.
public class FlashlightScript : MonoBehaviour
{

    GameObject raycaster;
    GameObject lights;

    Ray2D ray;
    RaycastHit2D hit;

    Vector3 dir;

    public LayerMask hitLayers;

    Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
        raycaster = transform.GetChild(0).gameObject;
        lights = transform.GetChild(1).gameObject;
    }

    void Update()
    {
        //Dispara un rallo.
        hit = Physics2D.Raycast(raycaster.transform.position, transform.up, Mathf.Infinity, hitLayers);

        //Si detecta una pared:
        if (hit == true && hit.collider.isTrigger == false) {

            //Comprueba su distancia.
            float distance = Vector2.Distance(raycaster.transform.position, hit.point);

            //Si esta más cerca que 0.7 metros:
            if (distance <= 0.7f)
            {
                //Se mueve hacia atrás cuanto más cerca este.
                float moveAmmount = Mathf.Abs(0.7f - distance);

                lights.transform.localPosition = Vector3.zero + (Vector3.up.normalized * -1 * moveAmmount);
            }
            //Si no es el caso retrocede a su posición estandard.
            else {
                lights.transform.localPosition = Vector3.zero;
            }
        }
    }
}
