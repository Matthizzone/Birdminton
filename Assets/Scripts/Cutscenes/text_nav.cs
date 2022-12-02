using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class text_nav : MonoBehaviour
{
    Input input;
    GameObject audio_manager;

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
    int advance_count = 500;

    private void Awake()
    {
        input = new Input();
        audio_manager = GameObject.Find("audio_manager");

        input.Gameplay.A.performed += ctx => A();
        //input.Gameplay.B.performed += ctx => B();
        //input.Gameplay.X.performed += ctx => X();
        //input.Gameplay.Y.performed += ctx => Y();

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

    private void Update()
    {
        if (advance_count < 400)
        {
            handle_scene_change();
        }
        else
        {
            if (transition)
            {
                handle_transition();
            }
            else
            {
                if (phrase_i >= phrases.Length || character >= phrases[phrase_i].Length)
                {
                    audio_manager.GetComponent<audio_manager>().Stop("text");
                }
                else if (character < 0.1f && phrase_i < phrases.Length)
                {
                    audio_manager.GetComponent<audio_manager>().Play("text");
                }

                if (phrase_i < phrases.Length)
                {
                    if (character < phrases[phrase_i].Length)
                    {
                        character += character_speed;
                    }

                    transform.Find("Textbox").GetComponent<TMPro.TMP_Text>().text = phrases[phrase_i].Substring(0, Mathf.RoundToInt(character));
                }
            }
        }
    }

    void A()
    {
        if (advance && advance_count > 400)
        {
            advance_count = 360;
            audio_manager.GetComponent<audio_manager>().Stop("text");
        }
        if (!transition)
        {
            phrase_i++;
            character = 0;
            transition = true;
            fadeout = 50;
            wait = phrase_i == 9 ? 150 : 50;
            if (phrase_i == 9) audio_manager.GetComponent<audio_manager>().Play("logo_new");
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
            transform.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1) * val;
            transform.Find("Textbox").GetComponent<TMPro.TMP_Text>().color = new Color(1, 1, 1) * val;
            return;
        }
        transform.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1) * 0;
        if (wait > 0)
        {
            wait--;
            return;
        }
        if (phrase_i == 9)
        {
            transform.Find("logo").GetComponent<RawImage>().color = new Color(1, 1, 1);
            transition = false;
            advance = true;
            return;
        }
        transform.Find("RawImage").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Sprites/intro" + (phrase_i + 1));
        transform.Find("Textbox").GetComponent<TMPro.TMP_Text>().text = "";
        transform.Find("Textbox").GetComponent<TMPro.TMP_Text>().color = new Color(1, 1, 1);

        if (phrase_i == 8)
        {
            transform.Find("Textbox").localPosition = new Vector3(453, -32, 0);
            transition = false;
            transform.Find("RawImage").GetComponent<RawImage>().enabled = false;
            return;
        }

        if (fadein > 0)
        {
            val = 1 - (fadein / 50.0f);
            fadein--;
            transform.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1) * val;
            return;
        }
        transform.Find("RawImage").GetComponent<RawImage>().color = new Color(1, 1, 1);
        transition = false;
    }

    void handle_scene_change()
    {
        transform.parent.parent.Find("Curtain").GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Min(1.5f - Mathf.Abs(180 - advance_count) / 120f, 1));
        advance_count--;
        if (advance_count == 180)
        {
            LoadNextScene();
        }
        if (advance_count == 0)
        {
            Destroy(gameObject);
        }
    }

    void Start_down()
    {
        if (phrase_i < 9)
        {
            phrase_i = 9;
            character = 0;
            transition = true;
            fadeout = 50;
            wait = 150;
            audio_manager.GetComponent<audio_manager>().Play("logo_new");
            fadein = 50;
            audio_manager.GetComponent<audio_manager>().Stop("text");
        }
    }

    void LoadNextScene()
    {
        transform.Find("logo").GetComponent<RawImage>().enabled = false;
        audio_manager.GetComponent<audio_manager>().Stop("text");
        audio_manager.GetComponent<audio_manager>().Play("court_intro");

        transform.parent.parent.Find("black_bg").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        transform.Find("Curtain").GetComponent<Image>().color = new Color(0, 0, 0, 0);

        // load the court
        instantiate_prefab("Gym", Vector3.zero);
        audio_manager.GetComponent<audio_manager>().Play("gym_sound", 1, true);

        instantiate_prefab("Game", Vector3.zero);
        instantiate_prefab("Gathering", new Vector3(-4.5f, 0, -8f));

        GameObject.Find("Players").transform.Find("enemy_right").gameObject.SetActive(true);
        GameObject.Find("Players").transform.Find("enemy_left").gameObject.SetActive(true);
        GameObject.Find("Players").transform.Find("launcher").gameObject.SetActive(false);

        instantiate_UI("UI/Coach1", Vector3.zero, transform.parent);
    }

    void instantiate_prefab(string name, Vector3 where)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        newfab.name = name;
        newfab.transform.position = where;
    }

    void instantiate_UI(string name, Vector3 where, Transform parent)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        newfab.name = name;
        newfab.transform.position = where;
        newfab.transform.SetParent(parent);
        newfab.transform.localPosition = Vector3.zero;
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
