using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class marvin_behavior : MonoBehaviour
{
    // Temporaries

    public float temp1 = 0.1f;
    public float temp2 = 0.9f;

    // Global References

    Rigidbody rb;
    Animator anim;
    GameObject UI;
    audio_manager audio_manager;
    Transform model;

    // Internal System

    bool grounded = true;
    int swing_type = 0;
    int max_swing_commit = 50; // character param
    bool mishit = false;
    bool serving = false;
    float prev_dash = -10; // time stamp of previous dash
    float prev_swing = -10; // time stamp of previous swing
    bool prev_miss = false; // remembers if your previous swing was a miss
    float energy = 1; // 0 - 1
    Quaternion prev_head_rotation;
    bool right_court = true; // are you defending the right court?
    Vector3 move_dir;

    // Parameters

    float swing_endlag = 0.3f;
    float dash_endlag = 0.5f;
    float mishit_window = 0.5f; // time since last swing where mishit will happen

    // ON/OFF

    bool jump_enabled = true;
    bool dash_enabled = true;
    bool move_enabled = true;
    bool clear_enabled = true;
    bool drop_enabled = true;
    bool smash_enabled = true;
    bool energy_enabled = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        model = transform.Find("model");
        anim = model.GetComponent<Animator>();
    }

    void Start()
    {
        // Set global references

        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        UI = GameObject.Find("UI");

        // set needed values
        prev_head_rotation = model.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head").transform.rotation;
        right_court = transform.localPosition.x > 0;
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

        flat_vel += transform.right * move_dir.x * move_power * Time.deltaTime;
        flat_vel += transform.forward * move_dir.z * move_power * Time.deltaTime;
        flat_vel *= Mathf.Pow(friction, Time.deltaTime);

        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);

        // --------------------------------- ANIMATIONS ----------------------------------

        if (rb.velocity.magnitude > 0.1f || (swing_type != 3 && Time.time - prev_swing < swing_endlag)) // turning
        {
            // face where?
            Vector3 target_angle = rb.velocity;
            if (endlag_check_swing() && move_enabled)
            {
                if (swing_type == 0) target_angle = -transform.parent.forward; // forehand
                if (swing_type == 1) target_angle = transform.parent.forward; // backhand
                if (swing_type == 2) target_angle = -transform.parent.right; // clear
                if (swing_type == 4) target_angle = transform.parent.right; // jumpsmash

                if (right_court) target_angle *= -1;
            }


            // dampen
            target_angle = Vector3.RotateTowards(model.forward, target_angle, 12 * Time.deltaTime, 0);
            target_angle.y = 0;

            //apply
            model.LookAt(transform.position + target_angle);
        }

        anim.SetFloat("speed", rb.velocity.magnitude);



        // -------------------------------- UI UPDATES --------------------------------------

        GameObject energy_bar = UI.transform.Find("GameUI").Find(right_court ? "energy_right" : "energy_left").Find("bg").Find("Bar").gameObject;

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
        GameObject closest_active_shuttle = get_closest_active_shuttle();
        Vector3 look_pos = closest_active_shuttle != null ? closest_active_shuttle.transform.Find("model").position : transform.forward;

        // head
        Transform head = model.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("head");
        Vector3 look_dir = look_pos - head.position;

        // Apply angle limit
        look_dir = Vector3.RotateTowards(
            model.Find("Armature").Find("pelvis").Find("torso").Find("chest").forward,
            look_dir,
            Mathf.Deg2Rad * 45, // Multiply by Mathf.Deg2Rad here to convert degrees to radians
            0
        );

        // Apply damping
        head.rotation = Quaternion.Slerp(
            prev_head_rotation,
            Quaternion.LookRotation(look_dir, transform.up),
            1 - Mathf.Exp(-20 * Time.deltaTime)
        );
        prev_head_rotation = head.transform.rotation;

        // eye
        PointEye(head.Find("left_eye"), look_pos);
        PointEye(head.Find("right_eye"), look_pos);
    }

    void HitShuttle(Transform shuttle, Vector3 target_point, float v_y)
    {
        // get swing type
        if (serving)
        {
            swing_type = 5; // serve
        }
        else if (!grounded)
        {
            swing_type = 4; // jumpsmash
        }
        else if (shuttle.Find("model").localPosition.z < transform.localPosition.z - 0.3f)
        {
            swing_type = 0; // forehand
        }
        else if (shuttle.Find("model").localPosition.z > transform.localPosition.z + 0.3f)
        {
            swing_type = 1; // backhand
        }
        else if (shuttle.Find("model").localPosition.y > transform.localPosition.y + 0.9f)
        {
            swing_type = 2; // clear
        }
        else
        {
            swing_type = 3; // lift (kinda like default)
        }

        anim.SetInteger("type", swing_type);
        anim.SetTrigger("swing");

        if (mishit)
        {
            v_y = 20;
            audio_manager.PlayMany("mishit");
        }
        else
        {
            if (v_y < 0) audio_manager.PlayMany("hit hard");
            else audio_manager.PlayMany("hit medium");
        }

        if (!grounded) v_y -= 5; // bonus for jumping

        shuttle.GetComponent<shuttle_behavior>().set_towards_right(!right_court);
        shuttle.GetComponent<shuttle_behavior>().set_trajectory(shuttle.Find("model").localPosition, target_point, v_y, mishit);
    }

    void Whiff()
    {
        audio_manager.PlayMany("woosh");
        if (!grounded) anim.SetInteger("type", 4); // jumpsmash
        else
        {
            int random_type = Mathf.FloorToInt(Random.Range(1f, 3.999f)); // lift, forehand, backhand, but not clear, bc of the turnaround
            if (random_type == 2) random_type = 0;
            anim.SetInteger("type", random_type);
        }
        anim.SetTrigger("swing");
        prev_swing = Time.time - swing_endlag / 2; // only the half the endlag for whiffing
    }

    bool endlag_check_swing()
    {
        return Time.time - prev_swing < swing_endlag;
    }

    void PointEye(Transform eye, Vector3 where)
    {
        Vector3 look_vector = where - eye.position;

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

    GameObject get_closest_active_shuttle()
    {
        // get the closest shuttle to the hitbox that is in flight.
        float min_dist = float.PositiveInfinity;
        GameObject closest_active_shuttle = null;

        foreach (Transform child in GameObject.Find("Game").transform.Find("shuttles").transform)
        {
            float test_dist = Vector3.Distance(child.Find("model").position, transform.Find("hitbox").position);

            if (test_dist < min_dist && child.GetComponent<shuttle_behavior>().get_in_flight())
            {
                min_dist = test_dist;
                closest_active_shuttle = child.gameObject;
            }
        }

        return closest_active_shuttle;
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
        anim.SetTrigger("serve");
        GameObject new_shuttle = create_prefab("shuttle");
        new_shuttle.transform.parent = model.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("shoulder.l").Find("arm.l").Find("forearm.l").Find("forearm.l_end");
        new_shuttle.transform.localPosition = Vector3.zero;
        new_shuttle.transform.rotation = Quaternion.identity;
    }

    GameObject create_prefab(string name)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        int start_index = name.LastIndexOf('/') + 1;
        newfab.name = name.Substring(start_index, name.Length - start_index);
        return newfab;
    }

    // ------------------------------------------ FOR CONTROLLERS -------------------------------------

    public void MoveTowards(Vector3 direction)
    {
        direction.y = 0;
        if (direction.magnitude > 1) move_dir = direction.normalized;
        else move_dir = direction;
    }

    public void TryDash(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;
        direction.y = 0;
        direction = direction.normalized;

        if (dash_enabled && grounded && Time.time - prev_dash > dash_endlag)
        {
            anim.SetTrigger("dash");
            audio_manager.PlayMany("leap", 0.4f);

            rb.velocity += direction * Mathf.Min(1, energy / 0.4f) * 8; // dash power

            if (energy_enabled)
            {
                if (energy > 0.4f) energy -= 0.40f;
                else energy = 0;
            }

            prev_dash = Time.time;
        }
    }

    public void TryJump()
    {
        if (jump_enabled && grounded && !serving && energy > 0.2f)
        {
            rb.velocity *= 0.5f;
            rb.velocity = new Vector3(rb.velocity.x, 4f * Mathf.Min(1, energy / 0.4f), rb.velocity.z); // jump power
            anim.SetTrigger("jump");
            if (energy_enabled)
            {
                if (energy > 0.4f) energy -= 0.40f;
                else energy = 0;
            }
        }
    }

    public void TryHitShuttles(Vector3 target_point, float v_y)
    {
        if (serving)
        {
            serving = false;

            anim.SetInteger("type", 5); // serve
            anim.SetTrigger("swing");

            Transform left_hand = model.Find("Armature").Find("pelvis").Find("torso").Find("chest").Find("shoulder.l").Find("arm.l").Find("forearm.l").Find("forearm.l_end");

            Transform shuttle = left_hand.Find("shuttle");
            shuttle.SetParent(GameObject.Find("Game").transform.Find("shuttles"));
            shuttle.transform.localPosition = Vector3.zero;
            shuttle.transform.localRotation = Quaternion.identity;
            shuttle.GetComponent<shuttle_behavior>().set_towards_right(!right_court);
            shuttle.GetComponent<shuttle_behavior>().set_trajectory(transform.localPosition + Vector3.up, target_point, v_y, false);

            audio_manager.PlayMany("hit medium");

            return;
        }

        if (Time.time - prev_swing < swing_endlag) return;

        if (!mishit) // if it is already set to true, leave it be
        {
            mishit = !prev_miss && Time.time - prev_swing < mishit_window;
        }

        bool at_least_one = false;

        // loop through all the shuttles and if any are in range, hit em
        foreach (Transform child in GameObject.Find("Game").transform.Find("shuttles").transform)
        {
            // if 1) in range, 2) in flight, 3) towards my side, 4) across the net
            if (Vector3.Distance(child.Find("model").position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2
                && child.GetComponent<shuttle_behavior>().get_in_flight() && !(child.GetComponent<shuttle_behavior>().get_towards_right() ^ right_court)
                && (right_court ? 1 : -1) * child.Find("model").position.x > 0)
            {
                at_least_one = true;
                if (v_y < 0) // it takes energy to hit it downwards, maximum is -20
                {
                    if (energy > -v_y / 20)
                    {
                        energy += v_y / 20;
                    }
                    else
                    {
                        v_y = -energy * 20;
                    }
                }
                HitShuttle(child, target_point, v_y);
            }
        }

        if (!at_least_one) // missed all possible shuttles
        {
            Whiff();
        }

        // update internal state
        prev_swing = Time.time;
        prev_miss = at_least_one;
        mishit = false;

        rb.velocity *= 0;
    }

    public bool get_right_court()
    {
        return right_court;
    }
}
