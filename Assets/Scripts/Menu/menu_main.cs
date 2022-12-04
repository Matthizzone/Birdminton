using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_main : menu_controllable
{
    // Temporaries

    public float temp1 = 0.008f;

    // Global References

    audio_manager audio_manager;

    // Internal System

    int s = 0;

    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
    }

    public override void MoveByAngle(float angle)
    {
        int new_s = -1;

        if (s == 0)
        {
            if (225 < angle && angle < 315) // down
            {
                new_s = 1;
            }
            else if (45 < angle && angle < 135) // up
            {
                new_s = 2;
            }
        }
        else if (s == 1)
        {
            if (225 < angle && angle < 315) // down
            {
                new_s = 2;
            }
            else if (45 < angle && angle < 135) // up
            {
                new_s = 0;
            }
        }
        else if (s == 2)
        {
            if (225 < angle && angle < 315) // down
            {
                new_s = 0;
            }
            else if (45 < angle && angle < 135) // up
            {
                new_s = 1;
            }
        }


        if (new_s > -1 && new_s != s)
        {
            s = new_s;
            audio_manager.PlayMany("woosh");
        }
    }

    void Update()
    {
        Vector3 target_pos = transform.Find("Choices").GetChild(s).position;
        transform.Find("Cursor").position = Vector3.Lerp(transform.Find("Cursor").position, target_pos, 1 - Mathf.Pow(1e-08f, Time.deltaTime));
    }

    public override menu_controllable A_Pressed()
    {
        if (s == 0) // Story Mode
        {
            GameObject IS = create_prefab("UI/IntroStory");
            IS.transform.SetParent(transform.parent.parent.Find("CutsceneUI"));
            IS.transform.localPosition = Vector3.zero;

            gameObject.SetActive(false);

            return null;
        }
        else if (s == 1) // Exhibition
        {
            transform.parent.Find("Exhibition").gameObject.SetActive(true);
            gameObject.SetActive(false);

            audio_manager.PlayMany("hit medium");

            return transform.parent.Find("Exhibition").GetComponent<menu_exhibition>();
        }
        else if (s == 2) // Practice
        {
            gameObject.SetActive(false);
            return load_game("player", "launcher", false, s + 1);
        }

        return null;
    }
}
