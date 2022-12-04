using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_pause : menu_controllable
{
    // Global References

    audio_manager audio_manager;

    // Internal System

    int s = 0;
    bool paused = false;

    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
    }

    public override void MoveByAngle(float angle)
    {
        if (paused)
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


            if (new_s > -1 && new_s != s)
            {
                s = new_s;
                audio_manager.PlayMany("woosh");
            }
        }
    }

    void Update()
    {
        Vector3 target_pos = transform.Find("Choices").GetChild(s).position;
        transform.Find("Cursor").position = Vector3.Lerp(transform.Find("Cursor").position, target_pos, 1 - Mathf.Pow(1e-08f, Time.deltaTime));
    }

    public override menu_controllable A_Pressed()
    {
        if (paused)
        {
            if (s == 0) // Main Menu
            {
                transform.parent.parent.Find("GameUI").gameObject.SetActive(false);
                Destroy(GameObject.Find("Game"));
                Destroy(GameObject.Find("Gym"));

                transform.parent.Find("Main").gameObject.SetActive(true);
                gameObject.SetActive(false);

                paused = false;

                return transform.parent.Find("Main").GetComponent<menu_main>();
            }
        }

        return null;
    }

    public override void Start_Pressed()
    {
        paused = !paused;
        gameObject.SetActive(paused);
    }
}
