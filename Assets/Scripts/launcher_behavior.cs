using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher_behavior : MonoBehaviour
{
    GameObject shuttle;

    private float shot_countdown = 3;

    private void Start()
    {
        shuttle = GameObject.Find("shuttle");
    }

    void Update()
    {
        shot_countdown -= Time.deltaTime;
        if (shot_countdown <= 0)
        {
            Vector3 landing_spot = new Vector3(-3.5f, 0, 0);
            landing_spot.y = 0;
            shuttle.GetComponent<shuttle>().set_trajectory(transform.position, landing_spot, 15, false);
            shuttle.GetComponent<shuttle>().set_towards_left(true);
            shot_countdown = 3;
        }
    }
}
