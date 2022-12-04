using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class coach_1: MonoBehaviour
{
    Input input;
    audio_manager audio_manager;
    GameObject UI;

    public GameObject next_UI;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    string[,] lines = { {"Coach", "Didn’t make the basketball team, I see, I see..." },
                        {"Coach", "That’s ok, this sport is a little different." },
                        {"Coach", "Here, everyone makes the team!" },
                        {"Coach", "There are no spots, just a ladder." },
                        {"Coach", "When you win, you move up, and when you lose, you move down." },
                        {"Coach", "The top 7 players face off against the top 7 from other school teams." },
                        {"Coach", "Everyone else can scrimmage together, or come for support." },
                        {"Coach", "Have you played before?" },
                        {"Coach", "Great! Your first match will be against Jubert." },
                        {"Coop", "See you later man!" },
                        {"Coach", "And your first match will be against Hubert." },
                        {"Hubert", "Hello, I’m Hubert!" },
                        {"Hubert", "I’m new to the team this year too!" },
                        {"Hubert", "I’m on the bottom of the ladder, but I still try very hard." },
                        {"Hubert", "You can borrow one of my rackets for this game." },
                        {"Hubert", "Let’s go!" } };

    float char_i = 0;
    float character_speed = 0.4f;
    int line_i = 0;
    int camera_change = 1000;
    int circle_change = 1000;

    private void Awake()
    {
        input = new Input();
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();

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
        if (circle_change < 999)
        {
            // 150
            float sizeDelt = Mathf.Abs(48 * circle_change - 3600) - 1200;
            GameObject.Find("CircleMask").GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelt, sizeDelt);
            if (circle_change == 75)
            {
                // load up hubert stuff
                GameObject.Find("Gathering").transform.Find("cutscene_cam").gameObject.SetActive(false);
                GameObject.Find("Game").transform.Find("game_cam").gameObject.SetActive(true);
                GameObject.Find("UI").transform.Find("GameUI").gameObject.SetActive(true);
                Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();
                DepthOfField tmp;
                if (volume.profile.TryGet<DepthOfField>(out tmp))
                {
                    tmp.active = false;
                }
                GameObject.Find("Players").transform.Find("enemy_left").gameObject.SetActive(false);
                GameObject.Find("Players").transform.Find("enemy_right").gameObject.SetActive(false);

                GameObject.Find("Players").transform.Find("player").gameObject.SetActive(true);
                GameObject.Find("Players").transform.Find("player").GetComponent<marvin_behavior>().enable_some(false, false, false, false, false, false);
                GameObject.Find("Players").transform.Find("player").GetComponent<marvin_behavior>().begin_serve();

                GameObject.Find("Players").transform.Find("hubert").gameObject.SetActive(true);
            }
            if (circle_change == 0)
            {
                GameObject next_UI_new = Instantiate(next_UI);
                next_UI_new.transform.SetParent(transform.parent);
                next_UI_new.transform.position = new Vector3(960, 540, 0);
                audio_manager.Stop("gym_sound");
                audio_manager.Play("extra_perc", 0, true);
                audio_manager.Play("bass", 1, true);
                audio_manager.Play("drums", 0, true);
                audio_manager.Play("epiano", 0, true);
                audio_manager.Play("regpiano", 0, true);
                audio_manager.Play("synth", 0, true);
                audio_manager.Play("choir", 0, true);
                Destroy(gameObject);
            }
            circle_change--;
        }
        else
        {
            if (camera_change < 0)
            {
                if (char_i < 0.1f)
                    audio_manager.Play("text");
                else if (char_i >= lines[line_i, 1].Length)
                    audio_manager.Stop("text");

                if (char_i < lines[line_i, 1].Length)
                {
                    char_i += character_speed;
                }

                transform.Find("Dialogue").Find("Speaker").GetComponent<TMPro.TMP_Text>().text = lines[line_i, 0];
                transform.Find("Dialogue").Find("Speaker").GetComponent<TMPro.TMP_Text>().color = get_speaker_color(lines[line_i, 0]);
                transform.Find("Dialogue").Find("Line").GetComponent<TMPro.TMP_Text>().text = lines[line_i, 1].Substring(0, Mathf.RoundToInt(char_i));
            }
            else if (camera_change < 999)
            {
                GameObject.Find("DoubleCurtainTop").GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 600 - 20 * Mathf.Abs(camera_change - 30));
                GameObject.Find("DoubleCurtainBottom").GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 600 - 20 * Mathf.Abs(camera_change - 30));
                if (camera_change == 30)
                {
                    GameObject.Find("Gym").transform.Find("establishcam_pivot").gameObject.SetActive(false);
                    GameObject.Find("Gathering").transform.Find("cutscene_cam").gameObject.SetActive(true);

                    Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();
                    DepthOfField tmp;
                    if (volume.profile.TryGet<DepthOfField>(out tmp))
                    {
                        tmp.active = true;
                    }
                }
                if (camera_change == 0)
                {
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    audio_manager.Play("coach", 1, true);
                }
                camera_change--;
            }
        }
    }

    void A()
    {
        if (camera_change > 999)
        {
            camera_change = 60;
        }
        else if (camera_change < 0)
        {
            if (line_i < lines.Length / 2 - 1)
            {
                line_i++;
                char_i = 0;
                audio_manager.Stop("text");
                audio_manager.Play("text");
            }
            else
            {
                // TUTORIAL GAME TIME
                if (circle_change > 999)
                {
                    transform.Find("Dialogue").gameObject.SetActive(false);
                    audio_manager.Stop("text");
                    audio_manager.Stop("coach");
                    circle_change = 150;
                }
            }
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

    Color get_speaker_color(string speaker)
    {
        if (speaker.Equals("Coach")) return new Color(1, 0, 0);
        else if (speaker.Equals("Coop")) return new Color(0, 0, 1);
        else if (speaker.Equals("Hubert")) return new Color(0, 1, 1);
        else return new Color(1, 1, 1);
    }
}


