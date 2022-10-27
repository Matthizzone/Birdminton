using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class penguin_anim : MonoBehaviour
{
    public GameObject shuttle;

    public float temp1 = 0;
    public float temp2 = 0;

    Rigidbody rb;
    Animator anim;
    string current_state;

    Vector3 prev_vel = Vector3.zero;
    Quaternion tilt = Quaternion.Euler(0, 90, 0);

    Quaternion prev_head_rotation;

    private void Start()
    {
        rb = transform.parent.parent.GetComponent<Rigidbody>();
        anim = transform.GetComponent<Animator>();
        prev_head_rotation = transform.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head").transform.rotation;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        // -------------------------------------- ACCEL TILT -------------------------------------------

        float tilt_factor = 45;
        prev_vel.y = 0;
        tilt = Quaternion.Lerp(
            tilt,
            Quaternion.Euler((rb.velocity.x - prev_vel.x) * tilt_factor, 90, (rb.velocity.z - prev_vel.z) * tilt_factor),
            1 - Mathf.Exp(-10 * Time.deltaTime)
        );

        transform.parent.localRotation = tilt;

        prev_vel = rb.velocity;



        // -------------------------------------- HEAD AND EYES -------------------------------------------

        Transform head = transform.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head");
        Vector3 look_dir = shuttle.transform.position - head.position;

        // Apply angle limit
        look_dir = Vector3.RotateTowards(
            transform.right,
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

        // grounded check
        int layerMask = 0;
        layerMask = ~layerMask; // every layer

        RaycastHit floor_point;
        bool grounded = Physics.Raycast(transform.parent.parent.position + Vector3.up * 0.05f, Vector3.down, out floor_point, 0.1f, layerMask);
        anim.SetBool("grounded", grounded);



        // ------------------------------------- SWING CHECKS ------------------------------------------

        if (shuttle.GetComponent<shuttle>().get_towards_player())
        {
            Transform hitbox = transform.parent.parent.Find("hitbox");
            //float t_add = 0.5f;

            //Vector3 future_hitbox_loc = hitbox.position + rb.velocity * t_add / 3;
            //if (Vector3.Distance(shuttle.GetComponent<shuttle>().get_pos(Time.time + t_add), future_hitbox_loc) < 1.5f)
            if (Vector3.Distance(shuttle.transform.position, hitbox.position) < 3f)
                {
                // swing commit
                anim.SetTrigger("swing");
                anim.SetInteger("shot_type", 0);
            }
        }

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
