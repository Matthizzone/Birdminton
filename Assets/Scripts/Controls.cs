using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    Input input;
    Rigidbody rb;
    Animator anim;

    GameObject shuttle;
    GameObject UI;
    audio_manager audio_manager;

    public float temp1 = 0.1f;
    public float temp2 = 0.9f;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    bool grounded = true;
    int swing_commit = -1;
    int swing_commit_type = 0;
    int max_swing_commit = 50; // character param
    int a_pressed_ago = 0;
    int b_pressed_ago = 0;
    bool mishit = false;
    bool serving = true;
    float prev_dash = -10; // time stamp of previous dash
    float energy = 1; // 0 - 1

    bool jump_enabled = true;
    bool dash_enabled = true;
    bool move_enabled = true;
    bool clear_enabled = true;
    bool drop_enabled = true;
    bool smash_enabled = true;


    private void Awake()
    {
        input = new Input();
        rb = GetComponent<Rigidbody>();
        anim = transform.Find("penguin_tilt").Find("penguin_model").GetComponent<Animator>();
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        UI = GameObject.Find("UI");
        shuttle = GameObject.Find("shuttle");

        input.Gameplay.A.performed += ctx => A();
        input.Gameplay.B.performed += ctx => B();
        input.Gameplay.X.performed += ctx => X();
        input.Gameplay.Y.performed += ctx => Y();

        input.Gameplay.Start.performed += ctx => Start_down();
        input.Gameplay.Select.performed += ctx => Select();

        input.Gameplay.LeftStickUp.performed += ctx => left_stick.y = ctx.ReadValue<float>();
        input.Gameplay.LeftStickUp.canceled += ctx => left_stick.y = 0;
        input.Gameplay.LeftStickDown.performed += ctx => left_stick.y = -ctx.ReadValue<float>();
        input.Gameplay.LeftStickDown.canceled += ctx => left_stick.y = 0;
        input.Gameplay.LeftStickLeft.performed += ctx => left_stick.x = -ctx.ReadValue<float>();
        input.Gameplay.LeftStickLeft.canceled += ctx => left_stick.x = 0;
        input.Gameplay.LeftStickRight.performed += ctx => left_stick.x = ctx.ReadValue<float>();
        input.Gameplay.LeftStickRight.canceled += ctx => left_stick.x = 0;
        input.Gameplay.LeftStickPress.performed += ctx => LeftStickPress();

        input.Gameplay.RightStickUp.performed += ctx => right_stick.y = ctx.ReadValue<float>();
        input.Gameplay.RightStickUp.canceled += ctx => right_stick.y = 0;
        input.Gameplay.RightStickDown.performed += ctx => right_stick.y = -ctx.ReadValue<float>();
        input.Gameplay.RightStickDown.canceled += ctx => right_stick.y = 0;
        input.Gameplay.RightStickLeft.performed += ctx => right_stick.x = -ctx.ReadValue<float>();
        input.Gameplay.RightStickLeft.canceled += ctx => right_stick.x = 0;
        input.Gameplay.RightStickRight.performed += ctx => right_stick.x = ctx.ReadValue<float>();
        input.Gameplay.RightStickRight.canceled += ctx => right_stick.x = 0;
        input.Gameplay.RightStickPress.performed += ctx => RightStickPress();

        input.Gameplay.LT.performed += ctx => triggers.x = ctx.ReadValue<float>();
        input.Gameplay.LT.canceled += ctx => triggers.x = 0;
        input.Gameplay.LB.performed += ctx => LB();
        input.Gameplay.RT.performed += ctx => triggers.y = ctx.ReadValue<float>();
        input.Gameplay.RT.canceled += ctx => triggers.y = 0;
        input.Gameplay.RB.performed += ctx => RB();

        input.Gameplay.DUp.performed += ctx => D_up();
        input.Gameplay.DDown.performed += ctx => D_down();
        input.Gameplay.DLeft.performed += ctx => D_left();
        input.Gameplay.DRight.performed += ctx => D_right();
    }

    private void Update()
    {
        // ------------------------------ GROUND CHECK -------------------------------------

        // grounded check
        int layerMask = 0;
        layerMask = ~layerMask; // every layer

        RaycastHit floor_point;
        bool new_grounded = Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, out floor_point, 0.1f, layerMask);
        if (grounded && !new_grounded)
        {
            anim.SetTrigger("land");
        }
        grounded = new_grounded;



        // -------------------------------- MOVEMENT ---------------------------------
        anim.SetBool("serving", serving);
        if (serving)
        {
            shuttle.transform.position = transform.Find("penguin_tilt").Find("penguin_model").Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("shoulder.l").Find("arm.l").Find("forearm.l").Find("forearm.l_end").position;
        }

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
            if (drop_enabled && a_pressed_ago == 0) HitShuttle(new Vector3(1, 0, left_stick.y * 3), 5); // drop
        }
        if (b_pressed_ago > 0)
        {
            b_pressed_ago--;
            if (clear_enabled && b_pressed_ago == 0) HitShuttle(new Vector3(6, 0, left_stick.y * 3), 15); // clear
        }


        // -------------------------------- SWING COMMITMENT ---------------------------------

        if (shuttle.GetComponent<shuttle>().get_towards_left())
        {
            Transform hitbox = transform.Find("hitbox");
            float t_add = 0.2f;

            Vector3 future_hitbox_loc = hitbox.position + rb.velocity * t_add / 3;
            Vector3 future_shuttle_loc = shuttle.GetComponent<shuttle>().get_pos(Time.time + t_add);

            if (Vector3.Distance(future_shuttle_loc, future_hitbox_loc) < 1.5f &&
                shuttle.GetComponent<shuttle>().get_towards_left())
            {
                // swing commit
                if (swing_commit < 0)
                {
                    swing_commit = max_swing_commit;
                    audio_manager.Play("woosh", 0.5f);
                    if (grounded)
                    {
                        if (future_shuttle_loc.y > future_hitbox_loc.y + 1.16f)
                            swing_commit_type = 2;
                        else
                        {
                            if (rb.velocity.z < 0)
                                swing_commit_type = 0;
                            else
                                swing_commit_type = 1;
                        }
                        anim.SetInteger("shot_type", swing_commit_type);
                        anim.SetTrigger("swing");
                    }
                    else
                    {
                        anim.SetTrigger("smash");
                        swing_commit_type = 4;
                    }
                }
            }
        }

        if (swing_commit > -1)
        {
            swing_commit--;
            if (swing_commit == 0) swing_commit_type = 1;
        }

        // -------------------------------- UI UPDATES --------------------------------------

        GameObject energy_bar = UI.transform.Find("GameUI").Find("Energy").Find("bg").Find("Bar").gameObject;

        energy_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 400 * energy);

        Color bar_color = new Color(0, 1, 0); // R -> Y -> G lerp
        if (energy < 0.5f)
            bar_color =  Color.Lerp(new Color(1, 0, 0), new Color(1, 1, 0), energy * 2);
        else
            bar_color = Color.Lerp(new Color(1, 1, 0), new Color(0, 1, 0), energy * 2 - 1);

        energy_bar.GetComponent<Image>().color = bar_color;

        if (energy < 1) energy += Time.deltaTime * 0.1f;
    }

    void A()
    {
        if (b_pressed_ago > 0)
        {
            if (smash_enabled)
            {
                if (energy < 0.4f) mishit = true;
                else energy -= 0.40f;

                HitShuttle(new Vector3(3.5f, 0, left_stick.y * 3), -10); // smash\
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
                if (energy < 0.4f) mishit = true;
                else energy -= 0.40f;

                HitShuttle(new Vector3(3.5f, 0, left_stick.y * 3), -10); // smash
            }
            a_pressed_ago = 0;
        }
        else
            b_pressed_ago = 3;
    }

    void X()
    {
        if (dash_enabled && grounded && Time.time - prev_dash > 0.5f)
        {
            swing_commit_type = 4;
            anim.SetInteger("shot_type", swing_commit_type);
            anim.SetTrigger("swing");
            audio_manager.Play("leap", 0.4f);
            rb.velocity += new Vector3(left_stick.x, 0, left_stick.y) * Mathf.Min(1, energy / 0.4f) * 5; // dash power
            swing_commit = 20;
            prev_dash = Time.time;
            if (energy > 0.4f) energy -= 0.40f;
            else energy = 0;
        }
    }

    void Y()
    {
        if (jump_enabled && grounded && !serving && energy > 0.1f)
        {
            rb.velocity *= 0.5f;
            rb.velocity = new Vector3(rb.velocity.x, 4f * Mathf.Min(1, energy / 0.4f), rb.velocity.z); // jump power
            anim.SetTrigger("jump");
            if (energy > 0.4f) energy -= 0.40f;
            else energy = 0;
        }
    }

    void D_down()
    {
        print("D down");
    }

    void D_up()
    {
        print("D up");
    }

    void D_left()
    {
        print("D left");
    }

    void D_right()
    {
        print("D right");
    }

    void LeftStickPress()
    {
        print("left stick press");
    }

    void RightStickPress()
    {
        print("right stick press");
    }

    void LB()
    {
        print("LB");
    }

    void RB()
    {
        print("RB");
    }

    void Start_down()
    {
        print("Start down");
    }

    void Select()
    {
        print("Select down");
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        input.Gameplay.Disable();
    }

    void HitShuttle(Vector3 target_point, float v_y)
    {
        if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2
            && (shuttle.GetComponent<shuttle>().get_in_flight() || serving) && shuttle.GetComponent<shuttle>().get_towards_left())
        {
            if (swing_commit < 0)
            {
                anim.SetInteger("shot_type", 3);
                anim.SetTrigger("swing");
                swing_commit = 20;
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

    public int get_swing_commit()
    {
        return swing_commit;
    }

    public int get_swing_commit_type()
    {
        return swing_commit_type;
    }

    public bool get_serving()
    {
        return serving;
    }

    public void enable_some(bool jump, bool dash, bool move, bool clear, bool drop, bool smash)
    {
        jump_enabled = jump;
        dash_enabled = dash;
        move_enabled = move;
        clear_enabled = clear;
        drop_enabled = drop;
        smash_enabled = smash;
    }

    public void begin_serve()
    {
        serving = true;
    }
}
