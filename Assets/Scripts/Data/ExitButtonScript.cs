using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButtonScript : MonoBehaviour
{

    bool appear = false;

    TMPro.TMP_Text text;

    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
        StartCoroutine(appearTimer());
    }

    private void Update()
    {
        if (appear == true) {
            if (text.color.a < 1)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime);
            }
        }   
    }

    IEnumerator appearTimer() {
        yield return new WaitForSeconds(60f);
        appear = true;
    }

    public void Exit() {
        Application.Quit();
    }
}
