using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{

    void Start()
    {
        GetComponent<Image>().CrossFadeAlpha(0, 2f, true);
    }

}
