using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class coach_2: MonoBehaviour
{
    Input input;
    audio_manager audio_manager;
    Controls controls;
    GameObject shuttle;

    public GameObject next_UI;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    float characters = 0;
    float character_speed = 0.4f;
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
        shuttle = GameObject.Find("shuttle");
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
    }

    private void Update()
    {
        handle_transition();
        phase_frame_count++;
    }

    void handle_transition()
    {
        if (phase == 0) // text
        {
            a_enabled = fill_text("Coach", "I’m impressed, you picked that up quickly for a beginner!");
            return;
        }
        else if (phase == 1) // text
        {
            a_enabled = fill_text("Coach", "I’ll give you the 11th spot.");
            return;
        }
        else if (phase == 2) // text
        {
            a_enabled = fill_text("Coop", "What about me coach!");
            return;
        }
        else if (phase == 3) // text
        {
            a_enabled = fill_text("Coach", "You’ve earned yourself the 12th spot.");
            return;
        }
        else if (phase == 4) // text
        {
            a_enabled = fill_text("Coop", "Nice!");
            return;
        }
        else if (phase == 5) // text
        {
            a_enabled = fill_text("Grey", "What are ya doin' coach? These guys have no fundamentals.");
            return;
        }
        else if (phase == 6) // text
        {
            a_enabled = fill_text("Coach", "We all start somewhere.");
            return;
        }
        else if (phase == 7) // text
        {
            a_enabled = fill_text("Grey", "I’d be surprised if you don’t trip over your own shoelaces.");
            return;
        }
        else if (phase == 8) // text
        {
            a_enabled = fill_text("Coach", "HEY. This is not how we treat the team.");
            return;
        }
        else if (phase == 9) // text
        {
            a_enabled = fill_text("Grey", "Whatever, I’ll see ya all tomorrow.");
            return;
        }
        else if (phase == 10) // text
        {
            a_enabled = fill_text("Coach", "...");
            return;
        }
        else if (phase == 11) // text
        {
            a_enabled = fill_text("Coach", "Never mind him, he’s just eager.");
            return;
        }
        else if (phase == 12) // text
        {
            a_enabled = fill_text("Coach", "I’ll see you boys back here tomorrow.");
            return;
        }
        else if (phase == 13) // fade out
        {
            transform.parent.parent.Find("Curtain").GetComponent<Image>().color = new Color(0, 0, 0, phase_frame_count / 100f);
            audio_manager.Play("gym_sound", 1 - phase_frame_count / 100f, true);

            if (phase_frame_count == 100)
            {
                SceneManager.UnloadSceneAsync("Court");
                SceneManager.LoadScene("UI Only", LoadSceneMode.Additive);
                GameObject next_UI_new = Instantiate(next_UI);
                next_UI_new.transform.SetParent(transform.parent);
                next_UI_new.transform.position = new Vector3(960, 540, 0);
                audio_manager.Play("samanthas_theme", 1, true);
                audio_manager.Stop("gym_sound");
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

        transform.Find("Dialogue").Find("Speaker").GetComponent<TMPro.TMP_Text>().text = speaker;
        transform.Find("Dialogue").Find("Speaker").GetComponent<TMPro.TMP_Text>().color = get_speaker_color(speaker);

        transform.Find("Dialogue").Find("Line").GetComponent<TMPro.TMP_Text>().text = line.Substring(0, Mathf.RoundToInt(characters));

        return false;
    }

    Color get_speaker_color(string speaker)
    {
        if (speaker.Equals("Coach")) return new Color(1, 0, 0);
        else if (speaker.Equals("Coop")) return new Color(0, 0, 1);
        else if (speaker.Equals("Hubert")) return new Color(0, 1, 1);
        else if (speaker.Equals("Grey")) return new Color(0.4f, 0.4f, 0.4f);
        else return new Color(1, 1, 1);
    }

    void A()
    {
        if (a_enabled)
        {
            phase_frame_count = 0;
            characters = 0;
            a_enabled = false;

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


