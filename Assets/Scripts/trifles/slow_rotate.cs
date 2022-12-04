using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slow_rotate : MonoBehaviour
{
    public float speed = 0.01f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed);
    }
}
