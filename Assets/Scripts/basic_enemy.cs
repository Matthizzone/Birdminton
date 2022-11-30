using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class basic_enemy : MonoBehaviour
{
    GameObject shuttle;
    audio_manager audio_manager;
    GameObject UI;
    Animator anim;

    public bool right_court = false;

    private Rigidbody rb;

    int serve_countdown = 0; // 0: serve on first frame, -1 is don't serve on first frame (let birdie drop)
    int enemy_score = 0;
    int player_score = 0;
    bool to_me = false;
    int swing_commit = -1;
    int swing_commit_type;
    bool serving = false;
    float prev_dash = -10; // time stamp of previous dash
    float energy = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        UI = GameObject.Find("UI");
        shuttle = GameObject.Find("shuttle");
        anim = transform.Find("penguin_model").GetComponent<Animator>();
    }

    void Update()
    {
        to_me = shuttle.GetComponent<shuttle>().get_towards_left() ^ right_court;
        anim.SetBool("serving", false);

        // MOVE
        Vector3 foot_pos = transform.position;
        foot_pos.y = 0;
        Vector3 target_pos = to_me ? shuttle.GetComponent<shuttle>().get_land_point() : new Vector3(right_court ? 2.4f : -2.4f, 0, 0);
        target_pos = !shuttle.GetComponent<shuttle>().get_in_flight() && to_me ? new Vector3(right_court ? 2.4f : -2.4f, 0, 0) : target_pos; // go get the birdie
        
        
        Vector3 move_dir = target_pos - foot_pos;
        move_dir = move_dir.normalized;

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;

        float move_power = 20;
        float friction = 0.0035f;

        flat_vel += transform.right * move_dir.x * move_power * Time.deltaTime;
        flat_vel += transform.forward * move_dir.z * move_power * Time.deltaTime;
        if (Vector3.Distance(transform.position, target_pos) < 0.2f) flat_vel = Vector3.zero;
        flat_vel *= Mathf.Pow(friction, Time.deltaTime);

        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);
        
        anim.SetFloat("speed", rb.velocity.magnitude);


        // -------------------------------- UI UPDATES --------------------------------------

        GameObject energy_bar = UI.transform.Find("GameUI").Find("Energy Enemy").Find("bg").Find("Bar").gameObject;

        energy_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 400 * energy);

        Color bar_color = new Color(0, 1, 0); // R -> Y -> G lerp
        if (energy < 0.5f)
            bar_color = Color.Lerp(new Color(1, 0, 0), new Color(1, 1, 0), energy * 2);
        else
            bar_color = Color.Lerp(new Color(1, 1, 0), new Color(0, 1, 0), energy * 2 - 1);

        energy_bar.GetComponent<Image>().color = bar_color;

        if (energy < 1) energy += Time.deltaTime * 0.1f;

        // hit birdie
        if (to_me && shuttle.GetComponent<shuttle>().get_in_flight())
        {
            if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2)
            {
                if (shuttle.transform.position.y > transform.Find("hitbox").position.y + 1.16f && energy >= 0.4f)
                {
                    hit_shuttle(new Vector3(-4, 0, Random.Range(-3, 3)), -8); // smash
                    energy -= 0.4f;
                }
                else
                {
                    GameObject player = GameObject.Find("player");
                    float x = player.transform.position.x > -4 ? -6 : -2;
                    float[] z_choices = { -3, 3 };
                    float z = z_choices[Random.Range(0, 2)];
                    float v_y = player.transform.position.x > -4 ? 15 : 5;
                    hit_shuttle(new Vector3(x, 0, z), v_y); // clear or drop

                }
            }
            
            //smash get
            if (Vector3.Distance(shuttle.GetComponent<shuttle>().get_land_point(), transform.Find("hitbox").position) > transform.Find("hitbox").localScale.x
                && shuttle.GetComponent<shuttle>().get_land_time() - Time.time < 0.5f && Time.time - prev_dash > 0.5f)
            {
                anim.SetInteger("shot_type", swing_commit_type);
                anim.SetTrigger("swing");
                prev_dash = Time.time;
                Vector3 to_shuttle = shuttle.transform.position - transform.position;
                to_shuttle.y = 0;
                to_shuttle = to_shuttle.normalized;
                if (energy > 0.4f) energy -= 0.40f;
                else energy = 0;

                rb.velocity += to_shuttle * Mathf.Min(1, energy / 0.4f) * 5; // dash power
                audio_manager.Play("leap", 0.4f);
            }


        // -------------------------------- SWING COMMITMENT ---------------------------------

            Transform hitbox = transform.Find("hitbox");
            float t_add = 0.2f;

            Vector3 future_hitbox_loc = hitbox.position + rb.velocity * t_add / 3;
            Vector3 future_shuttle_loc = shuttle.GetComponent<shuttle>().get_pos(Time.time + t_add);

            if (Vector3.Distance(future_shuttle_loc, future_hitbox_loc) < 1.5f)
            {
                // swing commit
                if (swing_commit < 0)
                {
                    swing_commit = 50;
                    audio_manager.Play("woosh", 0.5f);
                    if (future_shuttle_loc.y > future_hitbox_loc.y + 1.16f)
                        swing_commit_type = 2;
                    else
                    {
                        if (rb.velocity.z < 0)
                            swing_commit_type = right_court ? 1 : 0;
                        else
                            swing_commit_type = right_court ? 0 : 1;
                    }
                    anim.SetInteger("shot_type", swing_commit_type);
                    anim.SetTrigger("swing");
                }
            }
        }
        if (swing_commit >= 0) swing_commit--;
    }

    void hit_shuttle(Vector3 target_point, float v_y)
    {
        if (shuttle.GetComponent<shuttle>().get_in_flight())
        {
            if (shuttle.transform.position.y < 0) shuttle.transform.position = transform.position + Vector3.up;
            shuttle.GetComponent<shuttle>().set_towards_left(right_court);
            shuttle.GetComponent<shuttle>().set_trajectory(
                shuttle.transform.position,
                target_point * (right_court ? 1 : -1),
                v_y,
                false);
            audio_manager.Play("hit soft", 1);
            shuttle.GetComponent<TrailRenderer>().enabled = true;
            shuttle.GetComponent<TrailRenderer>().Clear();
            shuttle.transform.Find("mishit_line").gameObject.SetActive(false);
            shuttle.transform.Find("mishit_line").GetChild(0).gameObject.GetComponent<TrailRenderer>().Clear();
        }
    }

    void hit_shuttle_random()
    {
        if (shuttle.transform.position.y < 0) shuttle.transform.position = transform.position + Vector3.up;
        shuttle.GetComponent<shuttle>().set_towards_left(right_court);
        shuttle.GetComponent<shuttle>().set_trajectory(
            shuttle.transform.position,
            new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f)) * (right_court ? 1 : -1),
            15,
            false);
        audio_manager.Play("hit soft", 1);
        shuttle.GetComponent<TrailRenderer>().enabled = true;
        shuttle.GetComponent<TrailRenderer>().Clear();
        shuttle.transform.Find("mishit_line").gameObject.SetActive(false);
        shuttle.transform.Find("mishit_line").GetChild(0).gameObject.GetComponent<TrailRenderer>().Clear();
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
}
