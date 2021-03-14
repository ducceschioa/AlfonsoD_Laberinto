using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//ESTE SCRIPT SIRVE PARA LA INTERACCIÓN CON PUERTAS.
public class DoorScript : MonoBehaviour
{
    public string requiredKey = "";
    bool open;

    Database database;
    SpriteRenderer SR;

    AudioSource AS;

    private void Awake()
    {
        database = GameObject.Find("_DATABASE").GetComponent<Database>();
        SR = GetComponentInChildren<SpriteRenderer>();
        GetComponent<BoxCollider2D>().enabled = true;
        AS = GetComponent<AudioSource>();
    }

    //Activa la puerta.
    public void Activate()
    {
        //Si la puerta no requiere llave, o requiere una llave y el jugador la posee:
        if ((requiredKey != "" && database.IsKeyOwned(requiredKey)) || requiredKey == "")
        {
            //Activa el sonido de apertura.
            AS.Play();
            //Comienza el proceso de desaparecer
            open = true;
            //Destruye la sombra de la puerta y el colisionador.
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(GetComponentInChildren<ShadowCaster2D>());
        }
    }

    private void Update()
    {
        if (open == true) {
            //Aumenta la transparencia de la puerta hasta que sea invisible
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, SR.color.a - Time.deltaTime);
            if (SR.color.a < 0f && AS.isPlaying == false) {
                //Una vez sea invisible y el sonido se haya acabado de ejecutar se destruye el objeto.
                Destroy(gameObject);
            }
        }
    }
}
