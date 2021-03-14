using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//SCRIPT DE EL BOTÓN DE LA LUZ.
public class SwitchScript : MonoBehaviour
{
    public Sprite offSwitch;
    public Sprite offMonitor;
    SpriteRenderer SR;

    bool activated = false;

    private void Awake()
    {
        SR = GetComponentInChildren<SpriteRenderer>();
    }

    public void Activate()
    {
        if (activated == false)
        {
            Database database = GameObject.Find("_DATABASE").GetComponent<Database>();

            //Al ser activado reproduce un sonido.
            GetComponent<AudioSource>().Play();
            activated = true;

            //Activa la luz global que ilumina toda la escena.
            GameObject.Find("WorldLight").GetComponent<Light2D>().enabled = true;

            //Desactiva las luces del jugador.
            GameObject[] playerLights = GameObject.FindGameObjectsWithTag("PlayerLights");
            SR.sprite = offSwitch;

            //Avisa a la base de datos que el nivel ha sido completado
            database.GetComponent<Database>().ActivatedSwitch();

            //Busca todos los monitores y apaga las pantallas.
            GameObject[] monitors = GameObject.FindGameObjectsWithTag("Monitor");

            for (int i = 0; i < monitors.Length; i++)
            {
                monitors[i].GetComponent<SpriteRenderer>().sprite = offMonitor;
                monitors[i].GetComponentInChildren<Light2D>().enabled = false;
            }

            for (int i = 0; i < playerLights.Length; i++)
            {
                playerLights[i].GetComponent<Light2D>().enabled = false;
            }

            database.HideInteractablePrompt();
            database.blockMessage = true;
        }
    }

}
