using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageScript : MonoBehaviour
{

    private void Awake()
    {
        Time.timeScale = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Time.timeScale = 1;

            Destroy(gameObject);
        }
    }
}
