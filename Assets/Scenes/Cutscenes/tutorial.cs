using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class tutorial : MonoBehaviour
{
    Input input;
    audio_manager audio_manager;
    Controls controls;
    GameObject shuttle;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    float characters = 0;
    float character_speed = 1; // change back to 0.4f
    int phase = 0;
    int phase_frame_count = 0;
    bool a_enabled = false;
    int success_count;

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
        controls = GameObject.Find("Players").transform.Find("player").GetComponent<Controls>();
        controls.enable_some(false, false, false, false, false, false);
    }

    private void Update()
    {
        handle_transition();

        if (phase == 2 && !shuttle.GetComponent<shuttle>().get_in_flight() && !shuttle.GetComponent<shuttle>().get_towards_left())
        {
            transform.Find("Dialogue").gameObject.SetActive(true);
            controls.enable_some(false, false, false, false, false, false);
            phase++;
        }

        phase_frame_count++;
    }

    void A()
    {
        if (a_enabled)
        {
            if (phase == 1 || phase == 3)
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, false, true, false, false);
            }
            if (phase == 5)
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, false, false, false);
            }
            if (phase == 7 || phase == 11 || phase == 13)
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, true, false, false);
            }
            if (phase == 15)
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, false, true, false);
            }
            if (phase == 19)
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, true, true, false);
            }
            phase_frame_count = 0;
            characters = 0;
            a_enabled = false;

            phase++;
        }
    }

    void handle_transition()
    {
        if (phase == 0) // text
        {
            a_enabled = fill_text("Hubert", "Let�s start with the basics.");
            return;
        }
        else if (phase == 1) // text
        {
            a_enabled = fill_text("Hubert", "Hit the shuttle over to me by pressing B.");
            return;
        }
        else if (phase == 2) // waiting for A
        {
            if (Mathf.FloorToInt(phase_frame_count / 40) % 2 == 1)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/joy1");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/joy2");

            return;
        }
        else if (phase == 3) // text
        {
            a_enabled = fill_text("Hubert", "Now, I�m going to hit the shuttle to you, and you try to hit it back to me by pressing B at the right time. Ready?");
            return;
        }
        else if (phase == 4) // counting successful hits
        {
            a_enabled = false;

            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0) success_count++;
                
                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    success_count = 0;
                }
                else GameObject.Find("enemy_right").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(-3f, 0, 0));
            }
        }
        else if (phase == 5) // text
        {
            a_enabled = fill_text("Hubert", "Now, we�re going to try moving and hitting. First try using the left stick to move around.");
            return;
        }
        else if (phase == 6) // move around
        {
            if (left_stick.magnitude > 0.5f) success_count++;
            if (success_count > 300)
            {
                phase++;
                transform.Find("Dialogue").gameObject.SetActive(true);
                controls.enable_some(false, false, false, false, false, false);
                success_count = 0;
            }
        }
        else if (phase == 7) // text
        {
            a_enabled = fill_text("Hubert", "I�m going to hit you some shuttles, see if you can get to them in time.");
            return;
        }
        else if (phase == 8) // move and get
        {
            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0) success_count++;
                
                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    success_count = 0;
                }
                else GameObject.Find("enemy_right").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -5), 0, Random.Range(-2, 2)));
            }
        }
        else if (phase == 9) // text
        {
            a_enabled = fill_text("Hubert", "Wow you�re doing great!");
            return;
        }
        else if (phase == 10) // text
        {
            a_enabled = fill_text("Hubert", "You can control the direction you�re hitting by tilting the left stick where you want the shuttle to go.");
            return;
        }
        else if (phase == 11) // text
        {
            a_enabled = fill_text("Hubert", "Try hitting the next shuttles to the top of my box by tilting up just as you hit B.");
            return;
        }
        else if (phase == 12) // top box
        {
            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0 && shuttle.transform.position.z > 0.5f) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    success_count = 0;
                }
                else GameObject.Find("enemy_right").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 13) // text
        {
            a_enabled = fill_text("Hubert", "Now try hitting the next shuttles to the bottom of my box.");
            return;
        }
        else if (phase == 14) // bottom box
        {
            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0 && shuttle.transform.position.z < -0.5f) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    success_count = 0;
                }
                else GameObject.Find("enemy_right").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 15) // text
        {
            a_enabled = fill_text("Hubert", "Very nice! You can also use A to hit, but that hit is a little different. Give it a try.");
            return;
        }
        else if (phase == 16) // drop shots
        {
            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    success_count = 0;
                }
                else GameObject.Find("enemy_right").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 17) // text
        {
            a_enabled = fill_text("Hubert", "Wow! You learned a lot quicker than I did. Maybe I�m a better teacher.");
            return;
        }
        else if (phase == 18) // text
        {
            a_enabled = fill_text("Coach", "I HEARD THAT.");
            return;
        }
        else if (phase == 19) // text
        {
            a_enabled = fill_text("Hubert", "Sorry!! Anyway, it�s time to put what you learned to the test. Try it all out together!");
            return;
        }
        else if (phase == 20) // free play
        {
            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    success_count = 0;
                }
                else GameObject.Find("enemy_right").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 21) // text
        {
            a_enabled = fill_text("Hubert", "WOW! I can�t believe how quickly you learned all that!");
            return;
        }
        else if (phase == 22) // circle fade
        {
            float sizeDelt = Mathf.Abs(48 * phase_frame_count - 3600) - 1200;
            GameObject.Find("CircleMask").GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelt, sizeDelt);
            if (phase_frame_count == 75)
            {
                GameObject.Find("Cameras").transform.Find("cutscene_cam").gameObject.SetActive(true);
                GameObject.Find("Cameras").transform.Find("game_cam").gameObject.SetActive(false);
                GameObject.Find("UI").transform.Find("Game").gameObject.SetActive(false);
                Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();
                DepthOfField tmp;
                if (volume.profile.TryGet<DepthOfField>(out tmp))
                {
                    tmp.active = false;
                }
                //GameObject.Find("Players").transform.Find("enemy_left").gameObject.SetActive(true);
                //GameObject.Find("Players").transform.Find("player").gameObject.SetActive(false);

            }
            if (phase_frame_count > 150)
            {
                // done!
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
        else return new Color(1, 1, 1);
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
