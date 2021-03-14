using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ESTE SCRIPT GESTIONA LOS BOTONES DEL MENU PRINCIPAL Y ASIGNA FUNCIONES.
public class MainMenuFunctions : MonoBehaviour
{
    //Script que contiene toda la información técnica de la aplicación.
    Settings settingsScript;

    GameObject controls, controlsBackground, controlsBack, controlsTitle, wasd, mouse, clicks;

    GameObject credits, creditsBackground, creditsBack, creditsTitle, creditsText;

    //Cerrar el juego.
    public void Quit() {
        Application.Quit();
    }

    //Empezar a jugar.
    public void Play() {
        //Se asegura que el nivel este puesto como 0 (Menu Principal)
        GetComponent<Database>().currentLevel = 0;
        GetComponent<Database>().lifes = 5;
        //Avisa a la base de datos de comenzar el juego.
        GetComponent<Database>().NextLevel();
    }

    void Start()
    {
        //Asigna el script Settings.
        if (settingsScript == null)
        {
            settingsScript = GameObject.Find("_DATABASE").GetComponent<Settings>();
        }

        //Busca el botón "FullscreenToggle" y le asigna el valor que está guardado en el sistema operativo.
        Toggle windowed = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();
        Mute(windowed.onValueChanged);
        windowed.isOn = settingsScript.Windowed();
        Unmute(windowed.onValueChanged);

        //Busca el botón "LightQualityToggle" y le asigna el valor que está guardado en el sistema operativo.
        Toggle light = GameObject.Find("LightQualityToggle").GetComponent<Toggle>();
        Mute(light.onValueChanged);
        light.isOn = settingsScript.LightQuality();
        Unmute(light.onValueChanged);

        controls = GameObject.Find("Controls");
        controlsBackground = GameObject.Find("ControlsBackground");
        controlsBack = GameObject.Find("ControlsBack");
        controlsTitle = GameObject.Find("ControlsTitle");
        wasd = GameObject.Find("WASD");
        mouse = GameObject.Find("Mouse");
        clicks = GameObject.Find("Clicks");

        CloseControls();

        credits = GameObject.Find("Credits");
        creditsBackground = GameObject.Find("CreditsBackground");
        creditsBack = GameObject.Find("CreditsBack");
        creditsTitle = GameObject.Find("CreditsTitle");
        creditsText = GameObject.Find("CreditsText");

        CloseCredits();
    }

    //Abre los controles
    public void OpenControls()
    {
        controlsBackground.SetActive(true);
        controlsBack.SetActive(true);
        controlsTitle.SetActive(true);
        wasd.SetActive(true);
        mouse.SetActive(true);
        clicks.SetActive(true);

    }

    //Cierra los controles
    public void CloseControls()
    {
        controlsBackground.SetActive(false);
        controlsBack.SetActive(false);
        controlsTitle.SetActive(false);
        wasd.SetActive(false);
        mouse.SetActive(false);
        clicks.SetActive(false);
    }

    //Abre los créditos
    public void OpenCredits()
    {
        creditsBackground.SetActive(true);
        creditsBack.SetActive(true);
        creditsTitle.SetActive(true);
        creditsText.SetActive(true);
    }

    //Cierra los créditos
    public void CloseCredits()
    {
        creditsBackground.SetActive(false);
        creditsBack.SetActive(false);
        creditsTitle.SetActive(false);
        creditsText.SetActive(false);
    }

    //Abrir página de la EMAV
    public void OpenEmavLink()
    {
        Application.OpenURL("https://www.emav.com");

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

    //Desilencia un botón para que al cambiar el valor no se active el "OnClick".
    public void Unmute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }

    //Conversor de Int a Bool.
    bool IntToBool(int value)
    {
        if (value == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Conversor de Bool a Int.
    int boolToInt(bool value)
    {
        if (value == false)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}
