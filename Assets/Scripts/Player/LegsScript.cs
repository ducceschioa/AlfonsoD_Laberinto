using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//ESTE SCRIPT SIRVE PARA GESTIONAR LA ANIMACIÓN Y LAS SOMBRAS DE LAS PIERNAS DEL JUGADOR.
public class LegsScript : MonoBehaviour
{
    PlayerControl PC;
    Animator anim;

    List<GameObject> shadowCasters;
    int currentShadowCaster = 1;

    [HideInInspector] public bool walking = false;

    void Awake()
    {
        PC = GetComponentInParent<PlayerControl>();
        anim = GetComponent<Animator>();

        shadowCasters = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            shadowCasters.Add(transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        //Si el jugador camina:
        if (walking == true)
        {
            //Mirar en que dirección está marcado en jugador que su personaje se mueva.
            Vector2 dir = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

            //Crea una rotación a partir de las direciónes marcadas.
            Quaternion rot = Quaternion.FromToRotation(dir, Vector3.right);

            //Asigna la rotación de las piernas para que sea igual:
            if (rot.eulerAngles.z == 0 && Input.GetAxis("Vertical") == -1f)
            {
                //Arregla un error si la rotación en la Z tiene un valor de -180.
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                transform.rotation = rot;
            }
        }
    }

    //Activa los calculos de rotación y comienza la animación de caminar.
    public void Walk() {
        if (walking == false)
        {
            walking = true;
            anim.SetBool("Walking", true);
        }
    }

    //Desactiva los calculos de rotación y comienza la animación de estar quieto.
    public void Idle() {
        walking = false;
        StopAllCoroutines();
        currentShadowCaster = 1;
        anim.SetBool("Walking", false);

        shadowCasters[0].GetComponent<ShadowCaster2D>().castsShadows = true;
        StartCoroutine(RemoveShadowsIdle());
       
    }

 
    //ESTA FUNCIÓN ES LLAMADA DESDE EL EDITOR CON LOS EVENTOS DE KEYFRAMES DE LA ANIMACIÓN DE CAMINAR (Es un poco rollo si quieres saber como va mejor te lo explico con un video).
    public void SwapShadowCaster() {
        if (currentShadowCaster == shadowCasters.Count) {
            currentShadowCaster = 1;
        }

        shadowCasters[currentShadowCaster].GetComponent<ShadowCaster2D>().castsShadows = true;

        StartCoroutine(RemoveShadowsChecking());

        //Si no pongo EXACTAMENTE  0.0911 las sombras y los frames se desincronizan. Debe de ser un bug del Unity.
        //yield return new WaitForSeconds(1f - time);

        //SwapShadowCaster();
    }

    //Cosas técnicas de las sombras.
    IEnumerator RemoveShadowsChecking()
    {
        yield return new WaitForFixedUpdate();
        shadowCasters[0].GetComponent<ShadowCaster2D>().castsShadows = false;
        for (int i = 1; i < shadowCasters.Count; i++)
        {
            if (i != currentShadowCaster)
            {
                shadowCasters[i].GetComponent<ShadowCaster2D>().castsShadows = false;
            }
        }
        currentShadowCaster++;
    }

    //Más cosas técnicas de las sombras.
    IEnumerator RemoveShadowsIdle() {
        yield return new WaitForFixedUpdate();
        for (int i = 1; i < shadowCasters.Count; i++)
        {
            shadowCasters[i].GetComponent<ShadowCaster2D>().castsShadows = false;
        }
    }
}
