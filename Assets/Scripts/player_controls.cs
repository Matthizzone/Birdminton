using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class player_controls : MonoBehaviour
{
    // Temporaries

    public float temp1 = 0.1f;
    public float temp2 = 0.9f;

    // Controller Input

    Input input;
    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    // Global References

    Rigidbody rb;
    Animator anim;
    GameObject shuttle;
    GameObject UI;
    audio_manager audio_manager;

    // Internal System

    bool grounded = true;
    int swing_type = 0;
    int max_swing_commit = 50; // character param
    int a_pressed_ago = 0;
    int b_pressed_ago = 0;
    bool mishit = false;
    bool serving = true;
    float prev_dash = -10; // time stamp of previous dash
    float prev_swing = -10; // time stamp of previous swing
    float energy = 1; // 0 - 1
    Quaternion prev_head_rotation;

    // Parameters

    float swing_endlag = 0.8f;
    float dash_endlag = 0.5f;

    // ON/OFF

    bool jump_enabled = true;
    bool dash_enabled = true;
    bool move_enabled = true;
    bool clear_enabled = true;
    bool drop_enabled = true;
    bool smash_enabled = true;
    bool energy_enabled = false;


    private void Awake()
    {
        input = new Input();

        input.Gameplay.A.performed += ctx => A();
        input.Gameplay.B.performed += ctx => B();
        input.Gameplay.X.performed += ctx => X();
        input.Gameplay.Y.performed += ctx => Y();

        //input.Gameplay.Start.performed += ctx => Start_down();
        //input.Gameplay.Select.performed += ctx => Select();

        input.Gameplay.LeftStickUp.performed += ctx => left_stick.y = ctx.ReadValue<float>();
        input.Gameplay.LeftStickUp.canceled += ctx => left_stick.y = 0;
        input.Gameplay.LeftStickDown.performed += ctx => left_stick.y = -ctx.ReadValue<float>();
        input.Gameplay.LeftStickDown.canceled += ctx => left_stick.y = 0;
        input.Gameplay.LeftStickLeft.performed += ctx => left_stick.x = -ctx.ReadValue<float>();
        input.Gameplay.LeftStickLeft.canceled += ctx => left_stick.x = 0;
        input.Gameplay.LeftStickRight.performed += ctx => left_stick.x = ctx.ReadValue<float>();
        input.Gameplay.LeftStickRight.canceled += ctx => left_stick.x = 0;
        //input.Gameplay.LeftStickPress.performed += ctx => LeftStickPress();

        input.Gameplay.RightStickUp.performed += ctx => right_stick.y = ctx.ReadValue<float>();
        input.Gameplay.RightStickUp.canceled += ctx => right_stick.y = 0;
        input.Gameplay.RightStickDown.performed += ctx => right_stick.y = -ctx.ReadValue<float>();
        input.Gameplay.RightStickDown.canceled += ctx => right_stick.y = 0;
        input.Gameplay.RightStickLeft.performed += ctx => right_stick.x = -ctx.ReadValue<float>();
        input.Gameplay.RightStickLeft.canceled += ctx => right_stick.x = 0;
        input.Gameplay.RightStickRight.performed += ctx => right_stick.x = ctx.ReadValue<float>();
        input.Gameplay.RightStickRight.canceled += ctx => right_stick.x = 0;
        //input.Gameplay.RightStickPress.performed += ctx => RightStickPress();

        input.Gameplay.LT.performed += ctx => triggers.x = ctx.ReadValue<float>();
        input.Gameplay.LT.canceled += ctx => triggers.x = 0;
        //input.Gameplay.LB.performed += ctx => LB();
        input.Gameplay.RT.performed += ctx => triggers.y = ctx.ReadValue<float>();
        input.Gameplay.RT.canceled += ctx => triggers.y = 0;
        //input.Gameplay.RB.performed += ctx => RB();

        //input.Gameplay.DUp.performed += ctx => D_up();
        //input.Gameplay.DDown.performed += ctx => D_down();
        //input.Gameplay.DLeft.performed += ctx => D_left();
        //input.Gameplay.DRight.performed += ctx => D_right();
    }

    void Start()
    {
        // Set global references

        rb = GetComponent<Rigidbody>();
        anim = transform.Find("model").GetComponent<Animator>();
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        UI = GameObject.Find("UI");
        shuttle = GameObject.Find("shuttle");

        // set needed values
        prev_head_rotation = transform.Find("model").Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head").transform.rotation;
    }

    void Update()
    {
        // ------------------------------ GROUND CHECK -------------------------------------

        bool ground_check = check_ground();
        if (!grounded && ground_check)
        {
            anim.SetTrigger("land");
        }
        grounded = ground_check;


        // -------------------------------- MOVEMENT ---------------------------------

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;

        Vector3 leftstick_unit_length = new Vector2(left_stick.x, left_stick.y);
        if (leftstick_unit_length.magnitude > 1) leftstick_unit_length = leftstick_unit_length.normalized;

        float move_power = 20;
        float friction = 0.0035f;

        if (serving)
        {
            move_power = 60;
            friction = 0.0000001f;
        }
        else if (!grounded)
        {
            move_power = 3;
            friction = 0.38f;
        }

        if (!move_enabled) move_power = 0;

        flat_vel += transform.right * leftstick_unit_length.x * move_power * Time.deltaTime;
        flat_vel += transform.forward * leftstick_unit_length.y * move_power * Time.deltaTime;
        flat_vel *= Mathf.Pow(friction, Time.deltaTime);

        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);

        // -------------------------------- SWINGING ---------------------------------

        if (a_pressed_ago > 0)
        {
            a_pressed_ago--;
            if (drop_enabled && a_pressed_ago == 0) TryHitShuttle(new Vector3(1, 0, left_stick.y * 3), 5); // drop
        }
        if (b_pressed_ago > 0)
        {
            b_pressed_ago--;
            if (clear_enabled && b_pressed_ago == 0) TryHitShuttle(new Vector3(6, 0, left_stick.y * 3), 15); // clear
        }

        // --------------------------------- ANIMATIONS ----------------------------------

        if (rb.velocity.magnitude > 0.1f || (swing_type != 3 && Time.time - prev_swing < swing_endlag)) // turning
        {
            // face where?
            Vector3 target_angle = rb.velocity;
            if (endlag_check_swing()) {
                if (swing_type == 0) target_angle = -Vector3.forward; // forehand
                if (swing_type == 1) target_angle = Vector3.forward; // backhand
                if (swing_type == 2) target_angle = -Vector3.right; // clear
                if (swing_type == 4) target_angle = Vector3.right; // jumpsmash
            }

            // dampen
            target_angle = Vector3.RotateTowards(transform.Find("model").forward, target_angle, 12 * Time.deltaTime, 0);
            target_angle.y = 0;

            //apply
            transform.Find("model").LookAt(transform.position + target_angle);
        }

        anim.SetFloat("speed", rb.velocity.magnitude);



        // -------------------------------- UI UPDATES --------------------------------------

        GameObject energy_bar = UI.transform.Find("GameUI").Find("Energy").Find("bg").Find("Bar").gameObject;

        energy_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 400 * energy);

        Color bar_color = new Color(0, 1, 0); // R -> Y -> G lerp
        if (energy < 0.5f)
            bar_color = Color.Lerp(new Color(1, 0, 0), new Color(1, 1, 0), energy * 2);
        else
            bar_color = Color.Lerp(new Color(1, 1, 0), new Color(0, 1, 0), energy * 2 - 1);

        energy_bar.GetComponent<Image>().color = bar_color;

        if (energy < 1) energy += Time.deltaTime * 0.1f;
    }

    private void LateUpdate()
    {
        // head

        Transform head = transform.Find("model").Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head");
        Vector3 look_dir = shuttle.transform.position - head.position;

        // Apply angle limit
        look_dir = Vector3.RotateTowards(
            transform.Find("model").Find("Armature").Find("pelvis").Find("torso").Find("chest").forward,
            look_dir,
            Mathf.Deg2Rad * 45, // Multiply by Mathf.Deg2Rad here to convert degrees to radians
            0
        );

        // Apply damping
        head.rotation = Quaternion.Slerp(
            prev_head_rotation,
            Quaternion.LookRotation(look_dir, transform.up),
            1 - Mathf.Exp(-5 * Time.deltaTime)
        );
        prev_head_rotation = head.transform.rotation;

        // eye
        PointEye(head.Find("left_eye"));
        PointEye(head.Find("right_eye"));
    }

    void A()
    {
        if (b_pressed_ago > 0)
        {
            if (smash_enabled)
            {
                if (energy_enabled)
                {
                    if (energy < 0.4f) mishit = true;
                    else energy -= 0.40f;
                }

                TryHitShuttle(new Vector3(3.5f, 0, left_stick.y * 3), -10); // smash\
            }
            b_pressed_ago = 0;
        }
        else
            a_pressed_ago = 3;
    }

    void B()
    {
        if (a_pressed_ago > 0)
        {
            if (smash_enabled)
            {
                if (energy_enabled)
                {
                    if (energy < 0.4f) mishit = true;
                    else energy -= 0.40f;
                }

                TryHitShuttle(new Vector3(3.5f, 0, left_stick.y * 3), -10); // smash
            }
            a_pressed_ago = 0;
        }
        else
            b_pressed_ago = 3;
    }

    void X()
    {
        if (dash_enabled && grounded && Time.time - prev_dash > dash_endlag)
        {
            anim.SetTrigger("dash");
            audio_manager.Play("leap", 0.4f);
            rb.velocity += new Vector3(left_stick.x, 0, left_stick.y) * Mathf.Min(1, energy / 0.4f) * 5; // dash power
            prev_dash = Time.time;
            if (energy_enabled)
            {
                if (energy > 0.4f) energy -= 0.40f;
                else energy = 0;
            }
        }
    }

    void Y()
    {
        if (jump_enabled && grounded && !serving && energy > 0.1f)
        {
            rb.velocity *= 0.5f;
            rb.velocity = new Vector3(rb.velocity.x, 4f * Mathf.Min(1, energy / 0.4f), rb.velocity.z); // jump power
            anim.SetTrigger("jump");
            if (energy_enabled) {
                if (energy > 0.4f) energy -= 0.40f;
                else energy = 0;
            }
        }
    }

    void TryHitShuttle(Vector3 target_point, float v_y)
    {
        if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2
            && (shuttle.GetComponent<shuttle>().get_in_flight() || serving) && shuttle.GetComponent<shuttle>().get_towards_left())
        {
            if (!endlag_check_swing())
            {
                // get swing type
                if (!grounded)
                {
                    swing_type = 4; // jumpsmash
                }
                else if (shuttle.GetComponent<shuttle>().get_pos(Time.time).z < transform.Find("hitbox").position.z - 0.3f)
                {
                    swing_type = 0; // forehand
                }
                else if (shuttle.GetComponent<shuttle>().get_pos(Time.time).z > transform.Find("hitbox").position.z + 0.3f)
                {
                    swing_type = 1; // backhand
                }
                else if(shuttle.GetComponent<shuttle>().get_pos(Time.time).y > transform.Find("hitbox").position.y + 0.9f)
                {
                    swing_type = 2; // clear
                }
                else
                {
                    swing_type = 3; // lift
                }

                anim.SetInteger("type", swing_type);
                anim.SetTrigger("swing");
                prev_swing = Time.time;
            }
            if (mishit)
            {
                v_y = 20;
                audio_manager.Play("mishit");
            }
            else
            {
                if (v_y < 0) audio_manager.Play("hit hard");
                else audio_manager.Play("hit medium");
            }
            shuttle.GetComponent<shuttle>().set_trajectory(shuttle.transform.position, target_point, v_y, mishit);
            shuttle.GetComponent<shuttle>().set_towards_left(false);

            // reset values
            mishit = false;
            serving = false;
        }
        else if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x
            && shuttle.GetComponent<shuttle>().get_towards_left())
        {
            mishit = true;
        }
    }

    bool endlag_check_swing()
    {
        return Time.time - prev_swing < swing_endlag;
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

    bool check_ground()
    {
        int layerMask = 0;
        layerMask = ~layerMask; // every layer

        RaycastHit floor_point;
        return Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, out floor_point, 0.1f, layerMask);
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        input.Gameplay.Disable();
    }
}
