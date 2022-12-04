using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_behavior : MonoBehaviour
{
    Vector3 landing_point; // landing point of shuttle that dictates camera interest

    void Update()
    {
        Vector3 target_pos = landing_point / 4 + new Vector3(0, 8, -10);

        transform.localPosition = Vector3.Lerp(transform.localPosition, target_pos, 1 - Mathf.Pow(0.008f, Time.deltaTime));
    }

    public void set_landing_point(Vector3 point)
    {
        landing_point = point;
    }
}
