using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mishit_rotation : MonoBehaviour
{
    public float low;
    public float high;
    public float vect_low;
    public float vect_high;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Random.Range(0, 360));
    }
}
