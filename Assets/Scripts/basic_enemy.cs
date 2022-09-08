using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_enemy : MonoBehaviour
{
    public GameObject shuttle;
    public GameObject audio_manager;

    private int serve_countdown = -1;

    void Update()
    {
        if (serve_countdown > 0)
        {
            serve_countdown--;
            if (serve_countdown <= 0)
            {
                serve_shuttle();
            }
        }

        if (shuttle.transform.position.y < 0f)
        {
            if (shuttle.transform.position.x < 0f)
            {
                if (serve_countdown <= 0) serve_countdown = 60;
            }
            else
            {
                serve_shuttle();
            }
        }
    }

    void serve_shuttle()
    {
        shuttle.GetComponent<shuttle>().set_towards_player(true);
        shuttle.GetComponent<shuttle>().set_trajectory(
            transform.position + Vector3.up * 2.5f,
            new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f)),
            15);
        audio_manager.GetComponent<audio_manager>().Play("launch", 0.2f);
        audio_manager.GetComponent<audio_manager>().Play("G");
    }
}
