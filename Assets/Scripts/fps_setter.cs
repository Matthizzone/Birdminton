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
        try
        {
            FPS = FPS.Substring(0, decimal_index + 3);
        }
        catch {}
        gameObject.GetComponent<TMPro.TMP_Text>().text = FPS;
    }
}
