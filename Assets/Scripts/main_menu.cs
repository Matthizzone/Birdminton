using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class main_menu : MonoBehaviour
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
    public GameObject intro_story;

    // Internal System
    int m = 0;
    int num_choices = 0;
    bool up = false;
    bool down = false;

    private void Awake()
    {
        input = new Input();

        input.Gameplay.A.performed += ctx => A();
        //input.Gameplay.B.performed += ctx => B();
        //input.Gameplay.X.performed += ctx => X();
        //input.Gameplay.Y.performed += ctx => Y();

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

    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();

        num_choices = transform.Find("Choices").childCount;
    }

    void Update()
    {
        // set state variables
        if (left_stick.y > 0.9f) // up
        {
            if (!up)
            {
                m--;
                if (m < 0) m = num_choices - 1;
                audio_manager.Play_SFX("menu_neutral");

                up = true;
            }
        }
        else
        {
            up = false;
        }

        if (left_stick.y < -0.9f) // down
        {
            if (!down)
            {
                m++;
                if (m >= num_choices) m = 0;
                audio_manager.Play_SFX("menu_neutral");

                down = true;
            }
        }
        else
        {
            down = false;
        }




        // appearance updates
        for (int i = 0; i < num_choices; i++)
        {
            transform.Find("Choices").GetChild(i).GetChild(0).GetComponent<Image>().color =
                new Color(0.5f, 0.7f, 0.8f, i == m ? 1 : 0);
        }
    }

    void A()
    {
        audio_manager.Play_SFX("menu_good");
        enabled = false;

        if (m == 0) // Story Mode
        {
            GameObject IS = Instantiate(intro_story);
            IS.transform.SetParent(transform.parent.Find("CutsceneUI"));
            IS.transform.localPosition = Vector3.zero;

            SceneManager.LoadScene("UI Only", LoadSceneMode.Additive);
        }
        else if (m == 1) // Level 9 CPU
        {
            SceneManager.LoadSceneAsync("Court", LoadSceneMode.Additive);
            GameObject.Find("Players").transform.Find("enemy_right").gameObject.SetActive(true);
            GameObject.Find("Players").transform.Find("launcher").gameObject.SetActive(false);
            transform.parent.Find("GameUI").gameObject.SetActive(true);
        }
        else if (m == 2) // Easy Serves
        {
            SceneManager.LoadSceneAsync("Court", LoadSceneMode.Additive);
            GameObject.Find("Players").transform.Find("enemy_right").gameObject.SetActive(false);
            GameObject.Find("Players").transform.Find("launcher").gameObject.SetActive(true);
            transform.parent.Find("GameUI").gameObject.SetActive(true);
        }

        Destroy(gameObject);
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



