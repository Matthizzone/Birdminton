using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Sam1 : MonoBehaviour
{
    Input input;
    audio_manager audio_manager;
    Controls controls;

    public GameObject next_UI;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    float characters = 0;
    float character_speed = 1; // change back to 0.4f
    int phase = 0;
    int phase_frame_count = 0;
    bool a_enabled = false;

    private void Awake()
    {
        input = new Input();

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

    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
    }

    private void Update()
    {
        handle_transition();
        phase_frame_count++;
    }

    void handle_transition()
    {
        if (phase == 0) // curtain fade out
        {
            transform.parent.parent.Find("Curtain").GetComponent<Image>().color = new Color(0, 0, 0, 1 - phase_frame_count / 100f);
            if (phase_frame_count == 100)
            {
                phase++;
            }
        }
        else if (phase == 1) // text
        {
            a_enabled = fill_text("Marvin", "How are you, birdie?");
            return;
        }
        else if (phase == 2) // text
        {
            a_enabled = fill_text("Samantha", "I’m ok, but I’m worried about you.");
            return;
        }
        else if (phase == 3) // text
        {
            a_enabled = fill_text("Marvin", "Why’s that?");
            return;
        }
        else if (phase == 4) // text
        {
            a_enabled = fill_text("Samantha", "You don’t seem to be listening to me today.");
            return;
        }
        else if (phase == 5) // text
        {
            a_enabled = fill_text("Marvin", "Sorry, I’m just so excited about practice tomorrow.");
            return;
        }
        else if (phase == 6) // text
        {
            a_enabled = fill_text("Samantha", "That’s all?");
            return;
        }
        else if (phase == 7) // text
        {
            a_enabled = fill_text("Marvin", "Well this Grey guy also said something that bugged me a little.");
            return;
        }
        else if (phase == 8) // text
        {
            a_enabled = fill_text("Samantha", "Ah that’s what it is.");
            return;
        }
        else if (phase == 9) // text
        {
            a_enabled = fill_text("Marvin", "Sorry about today. I promise I’ll be focused once I take this all in.");
            return;
        }
        else if (phase == 10) // text
        {
            a_enabled = fill_text("Samantha", "Ok, I trust you.");
            return;
        }
        else if (phase == 11) // text
        {
            a_enabled = fill_text("Marvin", "I love you!");
            return;
        }
        else if (phase == 12) // text
        {
            a_enabled = fill_text("Samantha", "Love you too!");
            return;
        }
        else if (phase == 13) // fade out
        {
            transform.parent.parent.Find("Curtain").GetComponent<Image>().color = new Color(0, 0, 0, phase_frame_count / 100f);
            audio_manager.Play("samanthas_theme", 1 - phase_frame_count / 100f, true);

            if (phase_frame_count == 100)
            {
                GameObject next_UI_new = Instantiate(next_UI);
                next_UI_new.transform.SetParent(transform.parent);
                next_UI_new.transform.position = new Vector3(960, 540, 0);
                transform.parent.parent.Find("Curtain").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                audio_manager.Stop("samanthas_theme");
                audio_manager.Play("credits", 1, true);
                Destroy(gameObject);
            }
        }
    }

    bool fill_text(string speaker, string line)
    {
        if (characters < line.Length)
        {
            audio_manager.Play("text");
            characters += character_speed;
        }
        else
        {
            audio_manager.Stop("text");
            return true;
        }

        transform.Find(speaker).GetComponent<TMPro.TMP_Text>().text = line.Substring(0, Mathf.RoundToInt(characters));

        return false;
    }

    void A()
    {
        if (a_enabled)
        {
            phase_frame_count = 0;
            characters = 0;
            a_enabled = false;
            transform.Find("Marvin").GetComponent<TMPro.TMP_Text>().text = "";
            transform.Find("Samantha").GetComponent<TMPro.TMP_Text>().text = "";

            phase++;
        }
    }

    void B()
    {
        print("b down");
    }

    void X()
    {
        print("x down");
    }

    void Y()
    {
        print("y down");
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
}


