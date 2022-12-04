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

    // Internal References

    marvin_behavior MB;

    // System Variables

    int a_pressed_ago = 0;
    int b_pressed_ago = 0;

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
        MB = GetComponent<marvin_behavior>();
    }

    void Update()
    {
        // -------------------------------- MOVEMENT ---------------------------------

        MB.MoveTowards(new Vector3(left_stick.x, 0, left_stick.y));



        // -------------------------------- SWINGING ---------------------------------

        if (a_pressed_ago > 0)
        {
            a_pressed_ago--;
            if (a_pressed_ago == 0) MB.TryHitShuttles(new Vector3(1, 0, left_stick.y * 3), 2); // drop
        }
        if (b_pressed_ago > 0)
        {
            b_pressed_ago--;
            if (b_pressed_ago == 0) MB.TryHitShuttles(new Vector3(6, 0, left_stick.y * 3), 15); // clear
        }
    }

    void A()
    {
        if (b_pressed_ago > 0)
        {
            MB.TryHitShuttles(new Vector3(3.5f, 0, left_stick.y * 3), -5); // smash
            b_pressed_ago = 0;
        }
        else
            a_pressed_ago = 3;
    }

    void B()
    {
        if (a_pressed_ago > 0)
        {
            MB.TryHitShuttles(new Vector3(3.5f, 0, left_stick.y * 3), -5); // smash
            a_pressed_ago = 0;
        }
        else
            b_pressed_ago = 3;
    }

    void X()
    {
        MB.TryDash(new Vector3(left_stick.x, 0, left_stick.y));
    }

    void Y()
    {
        MB.TryJump();
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
