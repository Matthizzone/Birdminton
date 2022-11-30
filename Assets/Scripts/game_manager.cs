using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour
{
    int left_score = 0;
    int right_score = 0;

    float shuttle_death = Mathf.Infinity;

    bool transitioning = false;
    float t_0 = 0;
    public float transition_length = 4;

    void Update()
    {
        if (transitioning) // transist
        {
            float scale = (Time.time - t_0) / transition_length;
            transform.Find("logo_transition").GetComponent<RectTransform>().sizeDelta = new Vector2(1920, scale * 1080 * 2);
            transform.Find("logo_transition").Find("birdminton").GetComponent<RectTransform>().localScale = Vector3.one * scale * 2;

            if (scale > 0.5f)
            {
                GameObject.Find("player").GetComponent<Controls>().begin_serve();
                transform.Find("logo_transition").GetComponent<Image>().color = new Color(1, 1, 1, 1 - (scale - 0.5f) * 2);
                transform.Find("logo_transition").Find("birdminton").GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 1, 1 - (scale - 0.5f) * 2);
            }

            if (scale > 1)
            {
                // end transition
                transitioning = false;
                transform.Find("logo_transition").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                transform.Find("logo_transition").Find("birdminton").GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 1, 0);
                shuttle_death = Mathf.Infinity;
                GameObject.Find("shuttle").GetComponent<shuttle>().set_towards_left(true);
            }
        }
        else // check for new transitions
        {
            if (Time.time > shuttle_death + 1.5f)
            {
                //begin transition
                NewTransition();
            }
        }
    }

    public void NewTransition()
    {
        transitioning = true;
        t_0 = Time.time;
        transform.Find("logo_transition").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        transform.Find("logo_transition").Find("birdminton").GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 1, 1);
    }

    public void AddScore(bool left)
    {
        if (left) left_score++;
        else right_score++;
        transform.Find("Score").Find("Score").GetComponent<TMPro.TMP_Text>().text = left_score + " - " + right_score;
    }

    public void ShuttleDied(bool left)
    {
        shuttle_death = Time.time;
        AddScore(left);
    }
}
