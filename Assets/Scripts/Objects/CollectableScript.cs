using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT DE LAS MONEDAS DEL JUEGO.
public class CollectableScript : MonoBehaviour
{
    bool disappear = false;
    float multiplier = 2.5f;

    SpriteRenderer SR;

    public int value;

    AudioSource[] ASs;

    private void Start() {
        SR = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(removeRigidBodyTimer());
        StartCoroutine(layerChangeTimer());
        ASs = GetComponents<AudioSource>();
    }

    //Esto sirve por si la moneda ha sido disparada que al cabo de un rato deje de tener un rigidbody.
    IEnumerator removeRigidBodyTimer() {
        yield return new WaitForSeconds(1f);
        Destroy(GetComponent<Rigidbody2D>());
    }

    //Creo que esto debería borrarlo pero lo voy a dejar por si acaso rompo algo.
    IEnumerator layerChangeTimer() {
        yield return new WaitForSeconds(2f);
        gameObject.layer = 8;
    }

    void Update()
    {
        //Hace que la moneda se haga invisible.
        if (disappear == true) {
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, SR.color.a - Time.deltaTime * multiplier);
            if (SR.color.a < 0f) {
                if (ASs[0].isPlaying == false) {
                    //Cuando sea invisible se autodestruye.
                    Destroy(gameObject);
                }
            }
        }    
    }

    //Hace desaparecer la moneda.
    public void Disable(bool silent) {
        if (disappear == false) {
            if (silent == false)
            {
                ASs[0].Play();
            }
            disappear = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public bool isDisabled() {
        return disappear;
    }

    //Cuando al ser disparada choca contra otra moneda o un muro, hace un ruido de choque metálico.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ASs[1].Play();
    }
}
