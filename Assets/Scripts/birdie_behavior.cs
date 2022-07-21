using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdie_behavior : MonoBehaviour
{
    private Rigidbody rb;
    public float temp1 = 0.98f;
    

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    void Update()
    {
        rb.velocity *= temp1;
        transform.LookAt(transform.position + rb.velocity);
    }
}
