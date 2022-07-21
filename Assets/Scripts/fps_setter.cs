using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_setter : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        string FPS = "" + 1 / Time.deltaTime;
        int decimal_index = FPS.IndexOf(".");
        if (decimal_index > -1) FPS = FPS.Substring(0, decimal_index + 3);
        gameObject.GetComponent<TMPro.TMP_Text>().text = FPS;
    }
}
