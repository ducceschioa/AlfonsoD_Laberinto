using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//ESTE SCRIPT SIRVE PARA RELLENAR DE OPCIONES EL CAMBIO DE RESOLUCIÓN EN EL MENU PRINCIPAL.
public class GetResolutions : MonoBehaviour
{
    public List<Resolution> list;
    void Start()
    {
        //Asigna el desplegable de resoluciones.
        TMPro.TMP_Dropdown dropdown = GetComponent<TMPro.TMP_Dropdown>();

        dropdown.options.RemoveAt(0);

        list = new List<Resolution>();

        //Mira si en el sistema ya hay una configuración de resolución guardada.
        if (PlayerPrefs.HasKey("Width") == false)
        {
            PlayerPrefs.SetInt("Width", Screen.currentResolution.width);
            PlayerPrefs.SetInt("Height", Screen.currentResolution.height);
            PlayerPrefs.SetInt("Hz", Screen.currentResolution.refreshRate);
            PlayerPrefs.Save();
        }

        //Comprueba el ratio de la pantalla.
        float aspect = (float)Display.main.systemWidth / (float)Display.main.systemHeight;

        //rellena el desplegable con resoluciones compatibles.
        for (int i = Screen.resolutions.Length - 1; i > 0; i--)
        {
            string resText = Screen.resolutions[i].width + " x " + Screen.resolutions[i].height + ", " + Screen.resolutions[i].refreshRate + "Hz";
            TMPro.TMP_Dropdown.OptionData data = new TMPro.TMP_Dropdown.OptionData(resText);

            if (Mathf.Approximately((float)Screen.resolutions[i].width / (float)Screen.resolutions[i].height, aspect))
            {
                dropdown.options.Add(data);
                list.Add(Screen.resolutions[i]);
            }
        }

        //Muestra la resolución actual como la seleccionada en el desplegable.
        bool found = false;

        for (int i = 0; i < list.Count && found == false; i++)
        {
            if (list[i].width == Screen.currentResolution.width && list[i].height == Screen.currentResolution.height)
            {
                found = true;
                Mute(dropdown.onValueChanged);
                dropdown.value = i;
                Unmute(dropdown.onValueChanged);
            }

        }
    }

    //Silencia un botón para que al cambiar el valor no se active el "OnClick".
    public void Mute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
    }

    //De silencia un botón para que al cambiar el valor no se active el "OnClick".
    public void Unmute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }
}
