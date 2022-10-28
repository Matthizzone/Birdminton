using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class text_nav : MonoBehaviour
{
    Input input;

    public GameObject audio_manager;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    string[] phrases = { "Two penguin boys are best friends.",
                      "They are in their first year of high school, and they are excited to try out for the basketball team.",
                      "At tryouts, they are bested quickly by older students.",
                      "Bummed, they walk defeated back toward their lockers to change.",
                      "In the auxiliary gym, they see penguins playing a different sport.",
                      "They speak with a penguin who tells them what sport it is.",
                      "BADMINTON.",
                      "'This might be even better than basketball.'",
                      "'Let’s try out'" };
    float character = 0;
    float character_speed = 0.4f;
    int phrase_i = 0;

    bool transition = true;
    bool advance = false;
    int fadeout = 0;
    int wait = 50;
    int fadein = 50;

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

    private void Update()
    {
        if (transition)
        {
            handle_transition();
        }
        else {
            if (character < 0.1f)
                audio_manager.GetComponent<audio_manager>().Play("text");
            else if (character >= phrases[phrase_i].Length)
                audio_manager.GetComponent<audio_manager>().Stop("text");

            if (character < phrases[phrase_i].Length)
            {
                character += character_speed;
            }

            GetComponent<TMPro.TMP_Text>().text = phrases[phrase_i].Substring(0, Mathf.RoundToInt(character));
        }
    }

    void A()
    {
        if (advance)
        {
            SceneManager.LoadScene("Court", LoadSceneMode.Single);
        }
        if (!transition)
        {
            phrase_i++;
            character = 0;
            transition = true;
            fadeout = 50;
            wait = phrase_i == 9 ? 240 : 50;
            fadein = 50;
            audio_manager.GetComponent<audio_manager>().Stop("text");
        }
    }

    void handle_transition()
    {
        float val = 1;
        if (fadeout > 0)
        {
            val = fadeout / 50.0f;
            fadeout--;
            transform.parent.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1) * val;
            GetComponent<TMPro.TMP_Text>().color = new Color(1, 1, 1) * val;
            return;
        }
        transform.parent.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1) * 0;
        if (wait > 0)
        {
            wait--;
            return;
        }
        if (phrase_i == 9)
        {
            transform.parent.Find("logo").GetComponent<RawImage>().color = new Color(1, 1, 1);
            audio_manager.GetComponent<audio_manager>().Play("logo");
            transition = false;
            advance = true;
            return;
        }
        transform.parent.Find("RawImage").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Sprites/intro" + (phrase_i + 1));
        GetComponent<TMPro.TMP_Text>().text = "";
        GetComponent<TMPro.TMP_Text>().color = new Color(1, 1, 1);

        if (phrase_i == 8)
        {
            transform.localPosition = new Vector3(453, -32, 0);
            transition = false;
            return;
        }

        if (fadein > 0)
        {
            val = 1 - (fadein / 50.0f);
            fadein--;
            transform.parent.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1) * val;
            return;
        }
        transform.parent.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1);
        transition = false;
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
