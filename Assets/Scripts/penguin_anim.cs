using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class penguin_anim : MonoBehaviour
{
    public GameObject shuttle;

    public float temp1 = 0;
    public float temp2 = 0;

    Rigidbody rb;

    Vector3 prev_vel = Vector3.zero;
    Quaternion tilt = Quaternion.Euler(0, 90, 0);

    private void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        // -------------------------------------- ACCEL TILT -------------------------------------------
        float tilt_factor = 45;
        prev_vel.y = 0;
        tilt = Quaternion.Lerp(
            tilt,
            Quaternion.Euler((rb.velocity.x - prev_vel.x) * tilt_factor, 90, (rb.velocity.z - prev_vel.z) * tilt_factor),
            1 - Mathf.Exp(-10 * Time.deltaTime)
        );

        transform.localRotation = tilt;

        prev_vel = rb.velocity;

        // -------------------------------------- HEAD AND EYES -------------------------------------------
        Transform head = transform.Find("armature").Find("pelvis").Find("torso").Find("chest").Find("head");
        Vector3 look_dir = shuttle.transform.position - head.position;

        // Apply angle limit
        look_dir = Vector3.RotateTowards(
            transform.forward,
            look_dir,
            Mathf.Deg2Rad * 45, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
            0
        );

        // Apply damping
        head.rotation = Quaternion.Slerp(
            head.rotation,
            Quaternion.LookRotation(look_dir, transform.up),
            1 - Mathf.Exp(-5 * Time.deltaTime)
        );

        // eye time
        PointEye(head.Find("left_eye"));
        PointEye(head.Find("right_eye"));

    }

    void PointEye(Transform eye)
    {
        Vector3 look_vector = shuttle.transform.position - eye.position;



        float left_right_angle = Vector3.Angle(
            eye.up,
            Vector3.ProjectOnPlane(look_vector, eye.right)
        );

        left_right_angle *= Mathf.Sign(Vector3.Dot(eye.forward, look_vector));

        if (left_right_angle > 90) left_right_angle = 90; // make sure pupil stays in eye
        if (left_right_angle < -90) left_right_angle = -90;



        float up_down_angle = Vector3.Angle(
            eye.up,
            Vector3.ProjectOnPlane(look_vector, eye.forward)
        );

        up_down_angle *= Mathf.Sign(Vector3.Dot(eye.right, look_vector));

        if (up_down_angle > 90) up_down_angle = 90; // make sure pupil stays in eye
        if (up_down_angle < -90) up_down_angle = -90;
        


        Vector3 new_eye_position = new Vector3(up_down_angle / 90, 0, left_right_angle / 90);
        if (new_eye_position.magnitude > 1) new_eye_position /= new_eye_position.magnitude;
        eye.Find("pupil").localPosition = new_eye_position * 0.25f;
    }
}
