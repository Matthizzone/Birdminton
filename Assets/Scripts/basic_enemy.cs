using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_enemy : MonoBehaviour
{
    public GameObject shuttle;
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        UI = GameObject.Find("UI");
        anim = transform.Find("hubert").GetComponent<Animator>();
    }

    void Update()
    {
        to_me = shuttle.GetComponent<shuttle>().get_towards_left() ^ right_court;
        anim.SetBool("serving", false);

        // MOVE
        Vector3 foot_pos = transform.position;
        foot_pos.y = 0;
        Vector3 target_pos = to_me ? shuttle.GetComponent<shuttle>().get_land_point() : new Vector3(right_court ? 2.4f : -2.4f, 0, 0);
        target_pos = !shuttle.GetComponent<shuttle>().get_in_flight() && to_me ? shuttle.transform.position : target_pos; // go get the birdie
        
        
        Vector3 move_dir = target_pos - foot_pos;
        move_dir = move_dir.normalized;

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;

        float move_power = 0.3f;
        float friction = 0.91f;

        flat_vel += transform.right * move_dir.x * move_power;
        flat_vel += transform.forward * move_dir.z * move_power;
        if (Vector3.Distance(transform.position, target_pos) < 0.2f) flat_vel = Vector3.zero;
        flat_vel *= friction;

        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);
        
        anim.SetFloat("speed", rb.velocity.magnitude);


        // hit birdie
        if (to_me)
        {
            if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2)
            {
                hit_shuttle();
            }
        }


        // -------------------------------- SWING COMMITMENT ---------------------------------

        if (to_me)
        {
            Transform hitbox = transform.Find("hitbox");
            float t_add = 0.2f;

            Vector3 future_hitbox_loc = hitbox.position + rb.velocity * t_add / 3;
            Vector3 future_shuttle_loc = shuttle.GetComponent<shuttle>().get_pos(Time.time + t_add);

            print(Vector3.Distance(future_shuttle_loc, future_hitbox_loc));

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
        print(swing_commit);
    }

    void hit_shuttle()
    {
        if (shuttle.transform.position.y < 0) shuttle.transform.position = transform.position + Vector3.up;
        shuttle.GetComponent<shuttle>().set_towards_left(right_court);
        shuttle.GetComponent<shuttle>().set_trajectory(
            shuttle.transform.position,
            new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f)) * (right_court ? 1 : -1),
            15);
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
