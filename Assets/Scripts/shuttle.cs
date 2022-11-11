using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shuttle : MonoBehaviour
{
    public GameObject firework;
    Rigidbody rb;
    audio_manager audio_manager;

    public float temp1 = 5;
    public float temp2 = 360;

    float b = 2; // dissipation
    float g = 15; // gravity

    Vector3 r_0 = new Vector3(3, 0.5f, 0); // start point
    Vector3 r_f = new Vector3(0, 0, 0); // end point
    Vector2 v_0 = new Vector3(1, 1); // start velocity
    Vector3 angle = new Vector3(1, 0, 0);

    float t_0 = 0; // time of impact, beginning of flight

    Vector3 prev_loc = Vector3.zero; // store location from last from for lookat

    bool towards_left = true;
    bool in_flight = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
    }

    void Update()
    {
        // plug in t
        if (in_flight)
        {
            transform.position = get_pos(Time.time);

            Vector3 look_vector = transform.position - prev_loc;
            transform.LookAt(transform.position + look_vector);
            prev_loc = transform.position;

            if (get_pos(Time.time).y < 0.01f) kill_flight();
        }
    }

    public Vector3 get_pos(float t)
    {
        Vector3 floor_r_0 = new Vector3(r_0.x, 0, r_0.z);
        return floor_r_0 + angle * get_radius(t - t_0) + Vector3.up * get_height(t - t_0);
    }

    public void set_trajectory(Vector3 new_r_0, Vector3 new_r_f, float v_0_y)
    {
        // clean r_f
        r_f = new_r_f;
        r_f.y = 0;

        // update internal variables
        r_0 = new_r_0;
        v_0 = new Vector3(32, v_0_y);
        transform.position = r_0;

        Vector3 floor_r_0 = new Vector3(r_0.x, 0, r_0.z);
        float goal_radius = Vector3.Distance(floor_r_0, r_f);
        angle = (r_f - floor_r_0).normalized;

        // adjust v0 until the landing point is hit precisely
        int attempts = 10000;
        while (true)
        {
            float adj = Mathf.Pow(2, 10);
            while (Mathf.Abs(find_land_radius() - goal_radius) > 0.001f)
            {
                v_0.x += find_land_radius() < goal_radius ? adj : -adj;
                adj /= 2;
                if (adj < Mathf.Pow(2, -10)) break;
            }
            if (r_f.x * r_0.x > 0) break; // player and target loc are on same side of net, skip net check
            if (clearing_net()) break;
            if (attempts <= 0) break; // if you can't clear the net, so be it
            attempts--;
            v_0.y += 0.1f;
        }

        // initiate new flight
        t_0 = Time.time;

        GameObject.Find("dropspot").GetComponent<dropspot>().new_trajectory(find_height_zero() + Time.time, r_f);
        firework.transform.position = r_0;

        ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
        emitOverride.startLifetime = 0.5f;
        firework.GetComponent<ParticleSystem>().Emit(emitOverride, 150);
        in_flight = true;
        GetComponent<shuttle>().enabled = true;
        rb.velocity = Vector3.zero;
    }

    float get_radius(float t)
    {
        return v_0.x / b * (1 - Mathf.Exp(-b * t));
    }

    float get_radius_deriv(float t)
    {
        return v_0.x * Mathf.Exp(-b*t);
    }

    float inv_radius(float x)
    {
        // at what t does this x happen?
        // for x > v0.x / b will return NaN b/c it is the max distance the shuttle can travel.

        return -1 / b * Mathf.Log(1 - x * b / v_0.x);
    }

    float get_height(float t)
    {
        return r_0.y - g * t / b + (g / b + v_0.y) * (1 - Mathf.Exp(-b * t)) / b;
    }

    float get_height_deriv(float t)
    {
        return -g / b * (1 - Mathf.Exp(-b * t)) + v_0.y * Mathf.Exp(-b * t);
    }

    float find_height_zero()
    {
        // find the t value at which the shuttle hits the ground.

        float t = 0;
        if (v_0.y > 0)
            t = -1/b * Mathf.Log((g/b)/(v_0.y + g/b));
        t += 0.1f;

        while (Mathf.Abs(get_height(t)) > 0.0001f)
        {
            t -= get_height(t) / get_height_deriv(t);
        }

        return t;
    }

    float find_land_radius()
    {
        // how far will the shuttle travel in the x direction?

        return get_radius(find_height_zero());
    }

    bool clearing_net()
    {
        if (angle.x == 0) return true;

        float z_net = -angle.z / angle.x * r_0.x + r_0.z;

        Vector3 floor_r_0 = new Vector3(r_0.x, 0, r_0.z);
        Vector3 floor_net = new Vector3(0, 0, z_net);
        
        float net_dist = Vector3.Distance(floor_r_0, floor_net);

        if (net_dist >= v_0.x / b) return false; // the shuttle hits the floor before the net

        return get_height(inv_radius(net_dist)) > 1.7f; // net is actually 1.5, but want some wiggle room
    }

    public Vector3 get_land_point()
    {
        return r_f;
    }

    public float get_land_time()
    {
        return t_0 + find_height_zero();
    }

    public void set_towards_left(bool new_towards_player)
    {
        towards_left = new_towards_player;
    }
    public bool get_towards_left()
    {
        return towards_left;
    }

    private void kill_flight()
    {
        in_flight = false;
        GetComponent<shuttle>().enabled = false;

        Vector3 flat_vel = (get_pos(Time.time) - get_pos(Time.time - 0.1f)) * 10;
        flat_vel.y = -flat_vel.y;
        rb.velocity = flat_vel * 0.35f;

        if (Mathf.Abs(flat_vel.y) > 7) audio_manager.Play("bounce_hard", 0.4f);
        else audio_manager.Play("bounce_soft", 0.2f);

        rb.angularVelocity = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    public bool get_in_flight()
    {
        return in_flight;
    }
}
