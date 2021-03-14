using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//SCRIPT QUE CONTIENE Y ESCRIBE INFORMACIÓN AL SISTEMA OPERATIVO SOBRE EL JUEGO.
public class Settings : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.HasKey("LightQuality") == false)
        {
            PlayerPrefs.SetInt("LightQuality", 1);
        }
        if (PlayerPrefs.HasKey("Fullscreen") == false)
        {
            if (Screen.fullScreen == true)
            {
                PlayerPrefs.SetInt("Fullscreen", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Fullscreen", 0);
            }
        }

        if (PlayerPrefs.HasKey("Width") == false)
        {
            //Screen.SetResolution(1280, 720, true);
        }

        PlayerPrefs.Save();

        CheckIfValidResolution();
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        Time.timeScale = 1;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {

    }

    public void ChangeResolution()
    {
        TMPro.TMP_Dropdown dropdown = GameObject.Find("ResolutionDropdown").GetComponent<TMPro.TMP_Dropdown>();
        List<Resolution> list = dropdown.GetComponent<GetResolutions>().list;

        Resolution res = list[dropdown.value];

        bool fullscreen = Screen.fullScreen;
        Screen.SetResolution(res.width, res.height, fullscreen);

        PlayerPrefs.SetInt("Width", res.width);
        PlayerPrefs.SetInt("Height", res.height);
        PlayerPrefs.SetInt("Hz", res.refreshRate);
        PlayerPrefs.Save();

    }

    void CheckIfValidResolution()
    {
        float aspect = (float)Display.main.systemWidth / (float)Display.main.systemHeight;

        if (!Mathf.Approximately((float)Screen.width / (float)Screen.height, aspect))
        {
            bool found = false;
            for (int i = Screen.resolutions.Length - 1; i > 0 && found == false; i--)
            {
                if (Mathf.Approximately((float)Screen.resolutions[i].width / (float)Screen.resolutions[i].height, aspect))
                {
                    found = true;
                    Screen.SetResolution(Screen.resolutions[i].width, Screen.resolutions[i].height, Screen.fullScreen, Screen.resolutions[i].refreshRate);
                }
            }
        }
    }

    public bool Windowed()
    {
        if (Screen.fullScreen == false)
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
            return true;
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
            return false;
        }
    }

    public void ChangeWindowed()
    {
        if (Screen.fullScreen == false)
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
            Screen.fullScreen = true;
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
            Screen.fullScreen = false;
        }
    }

    public bool LightQuality()
    {
        if (PlayerPrefs.GetInt("LightQuality") == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ChangeLightQuality()
    {
        if (PlayerPrefs.GetInt("LightQuality") == 0)
        {
            PlayerPrefs.SetInt("LightQuality", 1);
        }
        else
        {
            PlayerPrefs.SetInt("LightQuality", 0);
        }
    }

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