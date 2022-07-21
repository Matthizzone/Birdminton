using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher_behavior : MonoBehaviour
{
    public GameObject birdie;
    public GameObject player;
    public GameObject star;
    public GameObject console;
    public float launch_power = 15f;

    private float shot_countdown = 3;

    void Update()
    {
        shot_countdown -= Time.deltaTime;
        if (shot_countdown <= 0)
        {
            Vector3 landing_spot = player.transform.position;
            landing_spot.y = 0;
            ShootBirdie(landing_spot, 10);
            shot_countdown = 5;
        }
    }

    void ShootBirdie(Vector3 landing_spot, float height)
    {
        float dist = Vector3.Distance(transform.position, landing_spot);
        Vector3 launch_angle = landing_spot - transform.position;
        launch_angle.y = 0;
        launch_angle = launch_angle.normalized;
        launch_angle += Vector3.up;
        launch_angle = launch_angle.normalized;

        birdie.transform.position = transform.position;
        birdie.GetComponent<Rigidbody>().velocity = launch_angle * 2 * dist;
        // new_birdie.GetComponent<birdie_behavior>().console = console;

        star.transform.position = landing_spot;

        gameObject.GetComponent<AudioSource>().Play();
    }
}
