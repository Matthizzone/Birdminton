using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_control : MonoBehaviour
{
    // Temporaries

    //public float temp1 = 0.1f;
    //public float temp2 = 0.9f;

    // Controller Input

    Input input;
    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    // Global References

    audio_manager audio_manager;
    menu_controllable current_menu;

    // Internal System
    bool stick_out = false;

    private void Awake()
    {
        input = new Input();

        input.Gameplay.A.performed += ctx => A();
        //input.Gameplay.B.performed += ctx => B();
        //input.Gameplay.X.performed += ctx => X();
        //input.Gameplay.Y.performed += ctx => Y();

        input.Gameplay.Start.performed += ctx => Start_down();
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

    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        current_menu = transform.Find("Main").GetComponent<menu_main>();
    }

    void Update()
    {
        if (left_stick.magnitude > 0.9f)
        {
            if (!stick_out)
            {
                current_menu.MoveByVector(left_stick.normalized);
                stick_out = true;
            }
        }
        else
        {
            stick_out = false;
        }
    }

    void A()
    {
        menu_controllable next_menu = current_menu.A_Pressed();
        if (next_menu != null) current_menu = next_menu;
        else Destroy(gameObject); // Temporary solution for going to cutscenes
    }

    void Start_down()
    {
        current_menu.Start_Pressed();
    }






    /*

    void A()
    {
        audio_manager.Play_SFX("menu_good");

        if (s == 0)
        {
            if (y == 0) // Story Mode
            {
                enabled = false;

                GameObject IS = create_prefab("UI/IntroStory");
                IS.transform.SetParent(transform.parent.Find("CutsceneUI"));
                IS.transform.localPosition = Vector3.zero;

                Destroy(gameObject);
            }
            else if (y == 1) // Exhibition
            {
                transform.Find("Screens").GetChild(s).gameObject.SetActive(false);
                s++;
                transform.Find("Screens").GetChild(s).gameObject.SetActive(true);
            }
            else if (y == 2) // Practice
            {
                enabled = false;

                GameObject gym = create_prefab("Gym");
                gym.transform.position = Vector3.zero;
                gym.transform.Find("establishcam_pivot").gameObject.SetActive(false);

                GameObject game = create_prefab("Game");
                game.transform.position = Vector3.zero;
                game.GetComponent<game_manager>().set_transitions(false);
                game.transform.Find("game_cam").gameObject.SetActive(true);
                game.transform.Find("Players").Find("launcher").gameObject.SetActive(true);
                game.transform.Find("Players").Find("player").gameObject.SetActive(true);

                transform.parent.Find("GameUI").gameObject.SetActive(true);

                Destroy(gameObject);
            }
        }
        else if (s == 1)
        {
            GameObject gym = create_prefab("Gym");
            gym.transform.position = Vector3.zero;
            gym.transform.Find("establishcam_pivot").gameObject.SetActive(false);

            GameObject game = create_prefab("Game");
            game.transform.position = Vector3.zero;
            game.GetComponent<game_manager>().set_transitions(true);
            game.transform.Find("game_cam").gameObject.SetActive(true);
            game.transform.Find("Players").Find("enemy_right").gameObject.SetActive(true);
            game.transform.Find("Players").Find("player").gameObject.SetActive(true);
            game.transform.Find("Players").Find("player").GetComponent<marvin_behavior>().begin_serve();

            transform.parent.Find("GameUI").gameObject.SetActive(true);
        }
    }

    GameObject create_prefab(string name)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        int start_index = name.LastIndexOf('/') + 1;
        newfab.name = name.Substring(start_index, name.Length - start_index);
        return newfab;
    }
    */

    private void OnEnable()
    {
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        input.Gameplay.Disable();
    }
}



