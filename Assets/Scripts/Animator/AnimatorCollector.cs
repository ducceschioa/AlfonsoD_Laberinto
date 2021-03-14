using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT QUE RECOGE MONEDAS PARA EL ANIMADOR.
public class AnimatorCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponentInParent<AnimatorAI>().satisfied == false)
        {
            //Si la moneda es recogida el Animador estará satisfecho.
            if (collision.gameObject.layer == 13 || collision.gameObject.layer == 8)
            {
                collision.GetComponent<CollectableScript>().Disable(true);
                GetComponentInParent<AnimatorAI>().Satisfy();
            }
        }
    }
}
