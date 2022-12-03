using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher_behavior : MonoBehaviour
{
    GameObject audio_manager;

    float shot_interval = 0.1f;
    float prev_launch = 0;

    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager");
    }

    void Update()
    {
        if (Time.time - prev_launch > shot_interval)
        {
            prev_launch = Time.time;

            GameObject new_shuttle = create_prefab("shuttle");
            new_shuttle.transform.position = Vector3.zero;
            new_shuttle.transform.SetParent(GameObject.Find("Game").transform.Find("shuttles"));

            //Vector3 landing_spot = new Vector3(-3.5f, 0, 0);
            Vector3 landing_spot = new Vector3(-3.5f + Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            landing_spot.y = 0;
            new_shuttle.GetComponent<shuttle>().set_trajectory(transform.localPosition, landing_spot, 15, false);
            new_shuttle.GetComponent<shuttle>().set_towards_left(true);

            audio_manager.GetComponent<audio_manager>().PlayMany("launch");
        }
    }

    GameObject create_prefab(string name)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        int start_index = name.LastIndexOf('/') + 1;
        newfab.name = name.Substring(start_index, name.Length - start_index);
        return newfab;
    }
}
