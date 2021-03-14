using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//SCRIPT QUE GESTIONALA ANIMACIÓN Y SOMBRAS DE LAS PIERNAS DE LOS ANIMADORES. LAS FUNCIONES YA ESTÁN EXPLICADAS EN EL SCRIPT DE LAS PIERNAS DEL JUGADOR.
public class AnimatorLegsScript : MonoBehaviour
{
    [HideInInspector] public Animator anim;

    List<GameObject> shadowCasters;
    int currentShadowCaster = 1;

    void Awake()
    {
        anim = GetComponent<Animator>();

        shadowCasters = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            shadowCasters.Add(transform.GetChild(i).gameObject);
        }

        anim.SetBool("Walking", true);
    }

 
    //ESTA FUNCIÓN ES LLAMADA DESDE EL EDITOR CON LOS EVENTOS DE KEYFRAMES DE LA ANIMACIÓN DE CAMINAR
    public void SwapShadowCaster() {
        if (currentShadowCaster == shadowCasters.Count) {
            currentShadowCaster = 1;
        }

        shadowCasters[currentShadowCaster].GetComponent<ShadowCaster2D>().castsShadows = true;

        StartCoroutine(RemoveShadowsChecking());
    }

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

    IEnumerator RemoveShadowsIdle() {
        yield return new WaitForFixedUpdate();
        for (int i = 1; i < shadowCasters.Count; i++)
        {
            shadowCasters[i].GetComponent<ShadowCaster2D>().castsShadows = false;
        }
    }
}
