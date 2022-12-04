using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_exhibition : menu_controllable
{
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
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 1;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 4;
            }
        }
        else if (s == 1)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 2;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 0;
            }
        }
        else if (s == 2)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 3;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 1;
            }
        }
        else if (s == 3)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 4;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 2;
            }
        }
        else if (s == 4)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 0;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 3;
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
        transform.parent.Find("Difficulty").gameObject.SetActive(true);

        audio_manager.PlayMany("hit medium");

        return transform.parent.Find("Difficulty").GetComponent<menu_difficulty>();
    }
}
