using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    Input input;
    Rigidbody rb;

    public GameObject birdie;
    public GameObject star;
    public GameObject UI;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    bool grounded = true;
    bool swinging = false;

    public float temp1 = 0.2f;
    public float temp2 = 0.98f;

    private void Awake()
    {
        input = new Input();
        rb = GetComponent<Rigidbody>();

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
        // ------------------------------ gather info -------------------------------------

        // grounded check
        int layerMask = 0;
        layerMask = ~layerMask; // every layer

        RaycastHit floor_point;
        grounded = Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, out floor_point, 0.1f, layerMask);

        if (Mathf.Abs(right_stick.x) + Mathf.Abs(right_stick.y) > 0.5)
        {
            if (!swinging)
            {
                // NEW SWING
                swinging = true;
                HitBirdie(right_stick);
            }
        }
        else
        {
            swinging = false;
        }



        // -------------------------------- do stuff ---------------------------------

        Vector3 flat_vel = rb.velocity;
        Vector3 leftstick_unit_length = new Vector2(left_stick.x, left_stick.y);
        if (leftstick_unit_length.magnitude > 1) leftstick_unit_length = leftstick_unit_length.normalized;
        if (grounded)
        {
            flat_vel.y = 0;
            flat_vel += transform.right * leftstick_unit_length.x * 0.3f;
            flat_vel += transform.forward * leftstick_unit_length.y * 0.3f;
            flat_vel *= 0.91f;
        }
        else
        {
            flat_vel += transform.right * leftstick_unit_length.x * 0.05f;
            flat_vel += transform.forward * leftstick_unit_length.y * 0.05f;
            flat_vel *= 0.984f; // same terminal velocity
        }
        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);

        star.transform.GetChild(1).localScale = Vector3.one * 100 / Mathf.Abs(0.75f - Vector3.Distance(transform.Find("Heart").position, birdie.transform.position));
    }

    void A()
    {
        //HitBirdie(new Vector2(triggers.x - triggers.y, -1));
        birdie.GetComponent<shuttle>().set_trajectory(
            transform.position + Vector3.up * 2.5f,
            new Vector3(3, 0, 0),
            temp1);
        gameObject.GetComponent<AudioSource>().Play();
    }

    void B()
    {
        HitBirdie(new Vector2(triggers.x - triggers.y, 1));
    }

    void X()
    {
        print("x down");
    }

    void Y()
    {
        if (grounded)
        {
            rb.velocity *= 0.5f;
            rb.velocity = new Vector3(rb.velocity.x, 4f, rb.velocity.z);
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
        float quality = Vector3.Distance(transform.Find("Heart").position, birdie.transform.position);
        if (Mathf.Abs(0.75f - quality) < 0.1f)
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "perfect";
        else if (Mathf.Abs(0.75f - quality) < 0.2f)
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "great!";
        else if (Mathf.Abs(0.75f - quality) < 0.3f)
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "ok";
        else
        {
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "miss";
            return;
        }

        Vector3 target_point = new Vector3(4, 0, 0);

        star.transform.position = target_point;

        Vector3 launch_angle = target_point - transform.position;
        launch_angle.y = 0;
        launch_angle = launch_angle.normalized;
        launch_angle -= Vector3.up * 0.2f;
        launch_angle = launch_angle.normalized;

        birdie.GetComponent<Rigidbody>().velocity = launch_angle * 42;

        gameObject.GetComponent<AudioSource>().Play();
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

    void HitBirdie(Vector2 angle)
    {
        float quality = Vector3.Distance(transform.Find("Heart").position, birdie.transform.position);
        if (Mathf.Abs(0.75f - quality) < 0.1f)
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "perfect";
        else if (Mathf.Abs(0.75f - quality) < 0.3f)
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "great!";
        else if (Mathf.Abs(0.75f - quality) < 0.5f)
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "ok";
        else
        {
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "miss";
            return;
        }

        if (angle.x == 0) angle /= Mathf.Abs(angle.y);
        else if (angle.y == 0) angle /= Mathf.Abs(angle.x);
        else angle /= Mathf.Max(Mathf.Abs(angle.x), Mathf.Abs(angle.y));

        Vector3 target_point = new Vector3(
            4 + angle.y * 3,
            0,
            angle.x * 3);

        // calculate target point
        //Vector3 target_point = new Vector3(
        //    Mathf.Lerp(near_far_target.x, near_far_target.y, (angle.x + 1) / 2),
        //    0,
        //    Mathf.Lerp(side_target.x, side_target.y, (angle.z + 1) / 2)
        //);

        star.transform.position = target_point;

        Vector3 launch_angle = target_point - transform.position;
        launch_angle.y = 0;
        launch_angle = launch_angle.normalized;
        launch_angle += Vector3.up;
        launch_angle = launch_angle.normalized;

        birdie.GetComponent<Rigidbody>().velocity = launch_angle * 2 * Vector3.Distance(transform.position, target_point);

        gameObject.GetComponent<AudioSource>().Play();
    }
}
