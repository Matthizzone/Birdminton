using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropspot : MonoBehaviour
{
    shuttle shuttle;

    float _hit_time;
    float _land_time;

    void Start()
    {
        shuttle = GameObject.Find("shuttle").GetComponent<shuttle>();
    }

    void Update()
    {
        if (shuttle.get_in_flight())
        {
            if (_hit_time == 0) return;

            float scale = (Time.time - _hit_time) / (_land_time - _hit_time); // 0 -> 1

            transform.localScale = Vector3.one * 200 * (1 - scale);
            GetComponent<MeshRenderer>().material.color = new Color(0.55f, 0.95f, 1, scale);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.clear;
        }
    }

    public void new_trajectory(float land_time, Vector3 land_point)
    {
        _land_time = land_time;
        transform.localPosition = land_point;
        _hit_time = Time.time;
    }
}
