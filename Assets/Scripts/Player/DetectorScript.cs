using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT QUE RECOGE MONEDAS Y LLAVES PARA EL JUGADOR.
public class DetectorScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {

        //Si es una moneda la recoge.
        if (collision.GetComponent<CollectableScript>() != null)
        {
            CollectableScript CS = collision.GetComponent<CollectableScript>();
            if (CS.isDisabled() == false)
            {
                collision.GetComponent<CollectableScript>().Disable(false);
                GetComponentInParent<PlayerControl>().database.IncreasePoints(collision.GetComponent<CollectableScript>().value);
            }
        }
        //Si es una llave la recoge.
        else if (collision.GetComponent<KeyScript>() != null) {
            collision.GetComponent<KeyScript>().Pickup();
        }
    }
}
