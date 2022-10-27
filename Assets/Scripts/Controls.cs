using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    Input input;
    Rigidbody rb;

    public GameObject shuttle;
    public GameObject strings;
    public Camera camera;
    public GameObject UI;
    public GameObject audio_manager;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    bool grounded = true;
    int a_pressed_ago = 0;
    int b_pressed_ago = 0;

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



        // -------------------------------- do stuff ---------------------------------

        Vector3 flat_vel = rb.velocity;
        flat_vel.y = 0;

        Vector3 leftstick_unit_length = new Vector2(left_stick.x, left_stick.y);
        if (leftstick_unit_length.magnitude > 1) leftstick_unit_length = leftstick_unit_length.normalized;

        float move_power = 0.3f;
        float friction = 0.91f;

        if (!grounded)
        {
            move_power = 0.05f;
            friction = 0.984f;
        }

        flat_vel += transform.right * leftstick_unit_length.x * move_power;
        flat_vel += transform.forward * leftstick_unit_length.y * move_power;
        flat_vel *= friction;

        rb.velocity = new Vector3(flat_vel.x, rb.velocity.y, flat_vel.z);

        if (a_pressed_ago > 0)
        {
            a_pressed_ago--;
            if (a_pressed_ago == 0) HitShuttle(new Vector3(1, 0, left_stick.y * 3), 5, 100); // drop
        }
        if (b_pressed_ago > 0)
        {
            b_pressed_ago--;
            if (b_pressed_ago == 0) HitShuttle(new Vector3(6, 0, left_stick.y * 3), 15, 100); // clear
        }
    }

    void A()
    {
        if (b_pressed_ago > 0)
        {
            HitShuttle(new Vector3(3.5f, 0, left_stick.y * 3), -10, 100); // smash
            b_pressed_ago = 0;
        }
        else
            a_pressed_ago = 3;
    }

    void B()
    {
        if (a_pressed_ago > 0)
        {
            HitShuttle(new Vector3(3.5f, 0, left_stick.y * 3), -10, 100); // smash
            a_pressed_ago = 0;
        }
        else
            b_pressed_ago = 3;
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

    void HitShuttle(Vector3 target_point, float v_y, float quality)
    {
        if (Vector3.Distance(shuttle.transform.position, transform.Find("hitbox").position) < transform.Find("hitbox").localScale.x / 2
            && shuttle.GetComponent<shuttle>().get_towards_player())
        {
            shuttle.GetComponent<shuttle>().set_trajectory(shuttle.transform.position, target_point, v_y);
            shuttle.GetComponent<shuttle>().set_towards_player(false);
            audio_manager.GetComponent<audio_manager>().Play("C");
            audio_manager.GetComponent<audio_manager>().Play("smash", 0.2f);

            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "perfect!!";
        }
        else
        {
            UI.transform.Find("Quality").GetComponent<TMPro.TMP_Text>().text = "miss...";
        }
    }
}
