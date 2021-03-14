using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//SCRIPT DE LAS LLAVES DEL JUEGO.
public class KeyScript : MonoBehaviour
{
    public string type;

    bool disappear = false;
    float multiplier = 1f;

    SpriteRenderer SR;

    AudioSource AS;

    private void Start()
    {
        SR = GetComponentInChildren<SpriteRenderer>();
        AS = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Hace desaparecer la llave.
        if (disappear == true)
        {
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, SR.color.a - Time.deltaTime * multiplier);
            if (SR.color.a < 0f && AS.isPlaying == false)
            {
                //Cuando sea invisible se autodestruye.
                Destroy(gameObject);
            }
        }
    }

    //Recoje la llave y la añade a la base de datos.
    public void Pickup()
    {
        if (disappear == false)
        {
            AS.Play();
            Destroy(GetComponentInChildren<ShadowCaster2D>());
            GameObject.Find("_DATABASE").GetComponent<Database>().AddKey(type);
            disappear = true;
        }
    }
}
