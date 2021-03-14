using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreenScript : MonoBehaviour
{

    Database database;

    private void Awake()
    {
        database = GameObject.Find("_DATABASE").GetComponent<Database>();
    }

    //Rendirse y volver al menu principal
    public void GiveUp()
    {
        StartCoroutine(giveUpTimer());
        GameObject.Find("GiveUp").GetComponent<Button>().interactable = false;
        GameObject.Find("Retry").GetComponent<Button>().interactable = false;
    }

    //Reiniciar partida desde el primer nivel
    public void Retry()
    {
        StartCoroutine(retryTimer());
        GameObject.Find("GiveUp").GetComponent<Button>().interactable = false;
        GameObject.Find("Retry").GetComponent<Button>().interactable = false;
    }

    IEnumerator retryTimer()
    {
        GameObject.Find("DefeatScreenfade").GetComponent<Image>().CrossFadeAlpha(1, 1f, true);

        yield return new WaitForSecondsRealtime(2);

        database.coins = 10;
        database.lifes = 5;
        database.currentLevel = 0;
        database.changingLevel = false;
        database.restarting = false;
        Time.timeScale = 1;
        database.NextLevel();

    }

    IEnumerator giveUpTimer()
    {
        GameObject.Find("DefeatScreenfade").GetComponent<Image>().CrossFadeAlpha(1, 1f, true);

        yield return new WaitForSecondsRealtime(2);

        Destroy(database.gameObject);
        Destroy(GameObject.Find("GUI"));
        SceneManager.LoadScene("MainMenu");

    }
}
