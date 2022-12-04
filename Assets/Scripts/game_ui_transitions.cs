using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game_ui_transitions : MonoBehaviour
{
    bool transitioning = false;
    float t_0 = 0;
    float transition_length = 1;

    void Update()
    {
        if (transitioning) // transist
        {
            float scale = (Time.time - t_0) / transition_length;
            transform.Find("logo_transition").GetComponent<RectTransform>().sizeDelta = new Vector2(1920, scale * 1080 * 2);
            transform.Find("logo_transition").Find("birdminton").GetComponent<RectTransform>().localScale = Vector3.one * scale * 2;

            if (scale > 0.5f)
            {
                transform.Find("logo_transition").GetComponent<Image>().color = new Color(1, 1, 1, 1 - (scale - 0.5f) * 2);
                transform.Find("logo_transition").Find("birdminton").GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 1, 1 - (scale - 0.5f) * 2);
            }

            if (scale > 1)
            {
                // end transition
                transitioning = false;
                transform.Find("logo_transition").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                transform.Find("logo_transition").Find("birdminton").GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 1, 0);
            }
        }
    }

    public void new_transition()
    {
        transitioning = true;
        t_0 = Time.time;
        transform.Find("logo_transition").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        transform.Find("logo_transition").Find("birdminton").GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 1, 1);
    }
}
