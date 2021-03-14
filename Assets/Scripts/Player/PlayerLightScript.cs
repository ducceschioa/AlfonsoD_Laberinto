using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//SCRIPT QUE HACE QUE EL JUGADOR APUNTE CON SU LINTERNA AL RATÓN.
public class PlayerLightScript : MonoBehaviour
{
    int angleOffset = -90;

    [HideInInspector] public Quaternion lastRotation;

    void Update() {
        if (Mathf.Approximately(Time.timeScale, 1f))
        {
            Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            lastRotation = transform.rotation;
        }
    }
}
