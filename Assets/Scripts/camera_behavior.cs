using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_behavior : MonoBehaviour
{
    public GameObject shuttle;

    public float pull = 4;
    public float smoothing = 0.008f;


    void Update()
    {
        Vector3 target_pos = shuttle.GetComponent<shuttle>().get_land_point() / pull + new Vector3(0, 8, -10);

        transform.position = Vector3.Lerp(transform.position, target_pos, 1 - Mathf.Pow(smoothing, Time.deltaTime));
    }
}
