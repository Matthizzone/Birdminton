using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_enemy : MonoBehaviour
{
    public GameObject shuttle;
    GameObject audio_manager;
    public GameObject UI;

    public bool right_court = false;

    private Rigidbody rb;

    private int serve_countdown = 0; // 0: serve on first frame, -1 is don't serve on first frame (let birdie drop)
    private int enemy_score = 0;
    private int player_score = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio_manager = GameObject.Find("audio_manager");
    }

    void Update()
    {
        // MOVE
        Vector3 foot_pos = transform.position;
        foot_pos.y = 0;
        Vector3 target_pos = shuttle.GetComponent<shuttle>().get_towards_left() ^ right_court ? new Vector3(right_court ? 2.4f : -2.4f, 0, 0) : shuttle.GetComponent<shuttle>().get_land_point();
        Vector3 move_dir = target_pos - foot_pos;
        move_dir = move_dir.normalized;

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;

        float move_power = 0.3f;
        float friction = 0.91f;

        flat_vel += transform.right * move_dir.x * move_power;
        flat_vel += transform.forward * move_dir.z * move_power;
        flat_vel *= friction;

        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);


        // hit birdie
        if (!shuttle.GetComponent<shuttle>().get_towards_left() ^ right_court)
        {
            if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2)
            {
                hit_shuttle();
            }
        }

        if (shuttle.transform.position.y < 0f)
        {
            if (serve_countdown < 0)
            {
                serve_countdown = 60;

                //scoring
                if (shuttle.transform.position.x < 0f)
                {
                    enemy_score++;
                }
                else
                {
                    player_score++;
                }
                UI.transform.Find("Score").GetComponent<TMPro.TMP_Text>().text = "" + player_score + " - " + enemy_score;
            }
            else
            {
                serve_countdown--;
                if (serve_countdown < 0)
                {
                    hit_shuttle();
                }
            }
        }
    }

    void hit_shuttle()
    {
        if (shuttle.transform.position.y < 0) shuttle.transform.position = transform.position + Vector3.up;
        shuttle.GetComponent<shuttle>().set_towards_left(true ^ right_court);
        shuttle.GetComponent<shuttle>().set_trajectory(
            shuttle.transform.position,
            new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f)) * (right_court ? 1 : -1),
            15);
        audio_manager.GetComponent<audio_manager>().Play("hit soft", 1);
        shuttle.GetComponent<TrailRenderer>().enabled = true;
        shuttle.GetComponent<TrailRenderer>().Clear();
        shuttle.transform.Find("mishit_line").gameObject.SetActive(false);
        shuttle.transform.Find("mishit_line").GetChild(0).gameObject.GetComponent<TrailRenderer>().Clear();
    }
}
