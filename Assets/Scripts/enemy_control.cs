using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_control : MonoBehaviour
{
    marvin_behavior MB;
    int level = 1; //1-9
    bool good_positioning = true;

    void Start()
    {
        MB = GetComponent<marvin_behavior>();
    }

    void Update()
    {
        
        GameObject closest_active_shuttle = get_closest_active_shuttle();

        if (closest_active_shuttle == null)
        {
            // ------------------------------ POSITIONING -------------------------------
            if (good_positioning)
            {
                MB.MoveTowards(
                GameObject.Find("Game").transform.TransformPoint(new Vector3(2.5f, 0, 0)) - transform.position);
            }
            else
            {
                MB.MoveTowards(Vector3.zero);
            }
        }
        else
        {
            // showtime >:)
            Vector3 vec_to_shuttle_pos = GameObject.Find("Game").transform.TransformPoint(
                closest_active_shuttle.GetComponent<shuttle_behavior>().get_land_point()) - transform.position;

            Vector3 floor_pos = transform.position;
            floor_pos.y = 0;

            Vector3 shuttle_floor_pos = closest_active_shuttle.transform.Find("model").position;
            float shuttle_y = shuttle_floor_pos.y;
            shuttle_floor_pos.y = 0;

            // -------------------------------- MOVEMENT ---------------------------------

            MB.MoveTowards(vec_to_shuttle_pos);



            // -------------------------------- JUMPING ---------------------------------


            if (Vector3.Distance(floor_pos, shuttle_floor_pos) < transform.Find("hitbox").localScale.x * 0.2f && 2.5f < shuttle_y && shuttle_y < 4.2f)
            {
                float jump_likelihood = -1.5f + 0.45f * level;
                if (Random.Range(0f, 1f) < jump_likelihood)
                {
                    MB.TryJump();
                }
            }



            // -------------------------------- DASHING ---------------------------------

            if (transform.Find("hitbox").localScale.x * 0.7f < Vector3.Distance(floor_pos, shuttle_floor_pos)
                && Vector3.Distance(floor_pos, shuttle_floor_pos) < transform.Find("hitbox").localScale.x * 1.5f
                && shuttle_y < 1.5f) // below net height
            {
                float jump_likelihood = -2.1f + 0.45f * level;
                if (Random.Range(0f, 1f) < jump_likelihood)
                {
                    MB.TryDash(vec_to_shuttle_pos);
                }
            }



            // -------------------------------- SWINGING ---------------------------------

            // if 1) in range, 2) in flight, 3) towards my side, 4) across the net
            if (Vector3.Distance(closest_active_shuttle.transform.Find("model").position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2
                && closest_active_shuttle.GetComponent<shuttle_behavior>().get_in_flight()
                && !(closest_active_shuttle.GetComponent<shuttle_behavior>().get_towards_right() ^ MB.get_right_court())
                && (MB.get_right_court() ? 1 : -1) * closest_active_shuttle.transform.Find("model").localPosition.x > 0)
            {
                float miss_percentage = 0.95f - 0.025f * level; // Chance of missing each frame. 0.99f is trash. 0.9f is ok. 0.8f is great. 0.7f is GOD
                if (Random.Range(0f, 1f) > miss_percentage)
                {
                    Vector3 where = new Vector3(-3.5f, 0, 0);
                    float power = 15f;

                    if (level == 1)
                    {
                        where = new Vector3(3.5f, 0, 0);
                        power = Random.Range(15f, 19f);
                    }
                    else if (level == 2)
                    {
                        where = new Vector3(Random.Range(2f, 4f), 0, Random.Range(-1.5f, 1.5f));
                        power = Random.Range(12f, 18f);
                    }
                    else if (level == 3)
                    {
                        where = new Vector3(Random.Range(1.5f, 5f), 0, Random.Range(-2f, 2f));
                        power = Random.Range(9f, 17f);
                    }
                    else if (level == 4)
                    {
                        where = new Vector3(Random.Range(1f, 6f), 0, Random.Range(-2.3f, 2.3f));
                        power = Random.Range(5f, 16f);
                    }
                    else if (level == 5) // best random shots
                    {
                        where = new Vector3(Random.Range(0.6f, 6.7f), 0, Random.Range(-2.6f, 2.6f));
                        power = Random.Range(2f, 15f);
                    }
                    else if (level == 6) // drop and clear are distinct, but rough
                    {
                        bool deep = Random.Range(0f, 1f) > 0.5f;
                        where = new Vector3(deep ? 1.5f : 5f, 0, Random.Range(-2.6f, 2.6f));
                        power = deep ? 15f : 2f;
                    }
                    else if (level == 7) // drop and clear are distinct and polished, smashes possible
                    {
                        bool deep = Random.Range(0f, 1f) > 0.5f;
                        where = new Vector3(deep ? 1f : 6f, 0, Random.Range(-2.6f, 2.6f));
                        power = deep ? 15f : 2f;

                        bool smash = Random.Range(0f, 1f) > 0.9f;
                        if (smash)
                        {
                            where = new Vector3(3.5f, 0, Random.Range(-2.6f, 2.6f));
                            power = -5f;
                        }
                    }
                    else if (level == 8) // smashes likely. sloppy corner placement only
                    {
                        bool deep = Random.Range(0f, 1f) > 0.5f;
                        bool right = Random.Range(0f, 1f) > 0.5f;
                        where = new Vector3(deep ? 0.6f : 6.7f, 0, right ? -2f : 2f);
                        power = Random.Range(2f, 15f);

                        bool smash = Random.Range(0f, 1f) > 0.5f;
                        if (smash)
                        {
                            where = new Vector3(3.5f, 0, right ? -2f : 2f);
                            power = -5f;
                        }
                    }
                    else if (level == 9) // smash intelligently. perfect corner placement only
                    {
                        bool deep = Random.Range(0f, 1f) > 0.5f;
                        bool right = Random.Range(0f, 1f) > 0.5f;
                        where = new Vector3(deep ? Random.Range(6f, 6.7f) : Random.Range(0.6f, 1.2f), 0, right ? -2.6f : 2.6f);
                        power = deep ? 15f : 0f;

                        bool smash = closest_active_shuttle.transform.position.y > 2.5f;
                        if (smash)
                        {
                            where = new Vector3(3.5f, 0, right ? -2.6f : 2.6f);
                            power = -5f;
                        }
                    }

                    where.x *= MB.get_right_court() ? -1 : 1;

                    MB.TryHitShuttles(where, power);

                    float positioning_likelihood = -0.45f + 0.45f * level;
                    good_positioning = Random.Range(0f, 1f) < positioning_likelihood;

                    //MB.TryHitShuttles(new Vector3(MB.get_right_court() ? -3.5f : 3.5f, 0, 0), 15); // weak clear for level 1 CPU
                }
            }
        }
    }

    GameObject get_closest_active_shuttle()
    {
        // get the closest shuttle to the hitbox that is in flight.
        float min_dist = float.PositiveInfinity;
        GameObject closest_active_shuttle = null;

        foreach (Transform child in GameObject.Find("Game").transform.Find("shuttles").transform)
        {
            float test_dist = Vector3.Distance(child.Find("model").position, transform.Find("hitbox").position);

            if (test_dist < min_dist && child.GetComponent<shuttle_behavior>().get_in_flight() && !(child.GetComponent<shuttle_behavior>().get_towards_right() ^ MB.get_right_court()))
            {
                min_dist = test_dist;
                closest_active_shuttle = child.gameObject;
            }
        }

        return closest_active_shuttle;
    }

    public void set_level(int new_level)
    {
        level = new_level;
    }
}
