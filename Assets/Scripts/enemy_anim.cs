using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_anim : MonoBehaviour
{
    public GameObject shuttle;

    public float temp1 = 0;
    public float temp2 = 0;

    Rigidbody rb;
    Animator anim;
    bool swing_commit = false;
    public bool right_court = false;

    Vector3 prev_vel = Vector3.zero;
    Quaternion tilt = Quaternion.Euler(0, 90, 0);

    Quaternion prev_head_rotation;

    private void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
        anim = transform.GetComponent<Animator>();
        prev_head_rotation = transform.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head").transform.rotation;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        // --------------------------------------- TURNING ---------------------------------------------
        int shot_type = transform.parent.GetComponent<basic_enemy>().get_swing_commit_type();
        Vector3 target_angle = Vector3.right * (shot_type == 1 ^ right_court ? -1 : 1);
        if (transform.parent.GetComponent<basic_enemy>().get_swing_commit() < 0) target_angle = new Vector3(-rb.velocity.z, 0, rb.velocity.x);
        if (transform.parent.GetComponent<basic_enemy>().get_serving()) target_angle = Vector3.right;

        target_angle = Vector3.RotateTowards(transform.forward, target_angle, 0.2f, 0);
        target_angle.y = 0;

        /* more relaxed angle when not your turn
        if (!shuttle.GetComponent<shuttle>().get_towards_player())
        {
            float angle = Vector3.Angle(Vector3.right, target_angle) * Mathf.Sign(Vector3.Dot(Vector3.forward, target_angle));
            target_angle *= angle <= 180 && angle > 45 ? 1 : -1;
        } */
        
        transform.LookAt(transform.position + target_angle);

        // -------------------------------------- ACCEL TILT -------------------------------------------

        /*
        float tilt_factor = 45;
        prev_vel.y = 0;
        tilt = Quaternion.Lerp(
            tilt,
            Quaternion.Euler((rb.velocity.x - prev_vel.x) * tilt_factor, 90, (rb.velocity.z - prev_vel.z) * tilt_factor),
            1 - Mathf.Exp(-10 * Time.deltaTime)
        );

        transform.parent.localRotation = tilt;

        prev_vel = rb.velocity;
        */

        // -------------------------------------- HEAD AND EYES -------------------------------------------

        Transform head = transform.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head");
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
            prev_head_rotation,
            Quaternion.LookRotation(look_dir, transform.up),
            1 - Mathf.Exp(-5 * Time.deltaTime)
        );
        prev_head_rotation = head.transform.rotation;

        // eye time
        PointEye(head.Find("left_eye"));
        PointEye(head.Find("right_eye"));

        

        // --------------------------------------- ANIMATOR --------------------------------------------


        anim.SetFloat("speed", rb.velocity.magnitude);

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
