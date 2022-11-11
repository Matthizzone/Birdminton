using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_setter : MonoBehaviour
{
    public int FPS = 60;

    void Awake()
    {
        Application.targetFrameRate = FPS;
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
