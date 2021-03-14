using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT QUE HACE QUE EL TEXTO DEL MENSAJE FINAL DEL JUEGO SE MUEVA HACIA ARRIBA.
public class CreditsScript : MonoBehaviour
{
    bool move = true;

    void Update()
    {
        if (move == true) {
            transform.Translate((Vector3.up * 25) * Time.deltaTime, Space.Self);
        }
    }
}
