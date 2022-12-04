using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_difficulty : menu_controllable
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
                new_s = 8;
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
                new_s = 5;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 3;
            }
        }
        else if (s == 5)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 6;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 4;
            }
        }
        else if (s == 6)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 7;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 5;
            }
        }
        else if (s == 7)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 8;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 6;
            }
        }
        else if (s == 8)
        {
            if ((0 <= angle && angle < 45) || (315 < angle && angle < 360)) // right
            {
                new_s = 0;
            }
            else if (135 < angle && angle < 225) // left
            {
                new_s = 7;
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
        GameObject gym = create_prefab("Gym");
        gym.transform.position = Vector3.zero;
        gym.transform.Find("establishcam_pivot").gameObject.SetActive(false);

        GameObject game = create_prefab("Game");
        game.transform.position = Vector3.zero;
        game.GetComponent<game_manager>().set_transitions(true);
        game.transform.Find("game_cam").gameObject.SetActive(true);
        game.transform.Find("Players").Find("enemy_right").gameObject.SetActive(true);
        game.transform.Find("Players").Find("enemy_right").GetComponent<enemy_control>().set_level(s + 1);
        game.transform.Find("Players").Find("player").gameObject.SetActive(true);
        game.transform.Find("Players").Find("player").GetComponent<marvin_behavior>().begin_serve();

        gameObject.SetActive(false);
        transform.parent.Find("Exhibition").gameObject.SetActive(false);
        transform.parent.parent.Find("GameUI").gameObject.SetActive(true);

        return transform.parent.Find("Pause").GetComponent<menu_pause>();
    }
}
