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

    public GameObject next_UI;

    Vector2 left_stick;
    Vector2 right_stick;
    Vector2 triggers;

    float characters = 0;
    float character_speed = 0.4f;
    int phase = 0;
    int phase_frame_count = 0;
    bool a_enabled = false;
    int success_count;
    bool shuttle_was_hit = false;

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
        phase_frame_count++;
    }

    void A()
    {
        if (a_enabled)
        {
            if (phase == 1) // hit B
            {
                shuttle.GetComponent<shuttle>().set_towards_left(true);
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, false, true, false, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(true);
            }
            else if (phase == 3) // time B
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, false, true, false, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(true);
            }
            else if (phase == 5) // move
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, false, false, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(true);
                audio_manager.Play("epiano", 1, true);
            }
            else if (phase == 7) // move and time B
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, true, false, false);
                audio_manager.Play("extra_perc", 1, true);
            }
            else if (phase == 11 || phase == 13) // top box, bottom box
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, true, false, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(true);
                audio_manager.Play("synth", 1, true);
            }
            else if (phase == 15) // hit A
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, false, true, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(true);
                audio_manager.Play("regpiano", 1, true);
            }
            else if (phase == 19) // full
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                controls.enable_some(false, false, true, true, true, false);
                audio_manager.Play("choir", 1, true);
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
            a_enabled = fill_text("Hubert", "Let’s start with the basics.");
            return;
        }
        else if (phase == 1) // text
        {
            a_enabled = fill_text("Hubert", "Hit the shuttle over to me by pressing B.");
            return;
        }
        else if (phase == 2) // waiting for B
        {
            if (Mathf.FloorToInt(phase_frame_count / 40) % 2 == 0)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/controller");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/B");

            if (!shuttle_was_hit && shuttle.GetComponent<shuttle>().get_in_flight()) shuttle_was_hit = true;

            if (shuttle_was_hit && !shuttle.GetComponent<shuttle>().get_in_flight())
            {
                transform.Find("Dialogue").gameObject.SetActive(true);
                controls.enable_some(false, false, false, false, false, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                audio_manager.Play("drums", 1, true);
                phase++;
            }

            return;
        }
        else if (phase == 3) // text
        {
            a_enabled = fill_text("Hubert", "Now, I’m going to hit the shuttle to you, and you try to hit it back to me by pressing B at the right time. Ready?");
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
                    transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                    success_count = 0;
                }
                else GameObject.Find("hubert").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(-3f, 0, 0));
            }
            
            if (phase_frame_count % 220 > 60 && phase_frame_count % 220 < 110)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/B");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/controller");
        }
        else if (phase == 5) // text
        {
            a_enabled = fill_text("Hubert", "Now, we’re going to try moving and hitting. First try using the left stick to move around.");
            return;
        }
        else if (phase == 6) // move around
        {
            if (left_stick.magnitude > 0.5f) success_count++;

            if (phase_frame_count % 80 > 40 && phase_frame_count % 80 < 80)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/joy1");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/joy2");

            if (success_count > 300)
            {
                phase++;
                transform.Find("Dialogue").gameObject.SetActive(true);
                controls.enable_some(false, false, false, false, false, false);
                transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                success_count = 0;
            }
        }
        else if (phase == 7) // text
        {
            a_enabled = fill_text("Hubert", "I’m going to hit you some shuttles, see if you can get to them in time.");
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
                    transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                    success_count = 0;
                }
                else GameObject.Find("hubert").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -5), 0, Random.Range(-2, 2)));
            }
        }
        else if (phase == 9) // text
        {
            a_enabled = fill_text("Hubert", "Wow you’re doing great!");
            return;
        }
        else if (phase == 10) // text
        {
            a_enabled = fill_text("Hubert", "You can control the direction you’re hitting by tilting the left stick where you want the shuttle to go.");
            return;
        }
        else if (phase == 11) // text
        {
            a_enabled = fill_text("Hubert", "Try hitting the next shuttles to the top of my box by tilting up just as you hit B.");
            return;
        }
        else if (phase == 12) // top box
        {
            if (phase_frame_count % 220 > 60 && phase_frame_count % 220 < 110)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/topbox");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/controller");

            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0 && shuttle.transform.position.z > 0.5f) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                    success_count = 0;
                }
                else GameObject.Find("hubert").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 13) // text
        {
            a_enabled = fill_text("Hubert", "Now try hitting the next shuttles to the bottom of my box.");
            return;
        }
        else if (phase == 14) // bottom box
        {
            if (phase_frame_count % 220 > 60 && phase_frame_count % 220 < 110)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/bottombox");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/controller");

            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {
                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0 && shuttle.transform.position.z < -0.5f) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                    success_count = 0;
                }
                else GameObject.Find("hubert").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 15) // text
        {
            a_enabled = fill_text("Hubert", "Very nice! You can also use A to hit, but that hit is a little different. Give it a try.");
            return;
        }
        else if (phase == 16) // drop shots
        {
            if (phase_frame_count % 220 > 60 && phase_frame_count % 220 < 110)
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/A");
            else
                transform.Find("Controller").GetComponent<RawImage>().texture = Resources.Load<Texture2D>("Controller/controller");

            if (Mathf.RoundToInt(phase_frame_count % 220) == 0)
            {

                if (shuttle.transform.position.y < 0.1f && shuttle.transform.position.x > 0) success_count++;

                if (success_count == 4) // you get one for free because of the first hit.
                {
                    phase++;
                    transform.Find("Dialogue").gameObject.SetActive(true);
                    controls.enable_some(false, false, false, false, false, false);
                    transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                    success_count = 0;
                }
                else GameObject.Find("hubert").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 17) // text
        {
            a_enabled = fill_text("Hubert", "Wow! You learned a lot quicker than I did. Maybe I’m a better teacher.");
            return;
        }
        else if (phase == 18) // text
        {
            a_enabled = fill_text("Coach", "I HEARD THAT.");
            return;
        }
        else if (phase == 19) // text
        {
            a_enabled = fill_text("Hubert", "Sorry!! Anyway, it’s time to put what you learned to the test. Try it all out together!");
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
                    transform.Find("Controller").GetComponent<RawImage>().gameObject.SetActive(false);
                    success_count = 0;
                }
                else GameObject.Find("hubert").GetComponent<helpful_hubert>().hit_shuttle(new Vector3(Random.Range(-2, -4), 0, Random.Range(-1, 1)));
            }
        }
        else if (phase == 21) // text
        {
            a_enabled = fill_text("Hubert", "WOW! I can’t believe how quickly you learned all that!");
            return;
        }
        else if (phase == 22) // circle fade
        {
            float sizeDelt = Mathf.Abs(48 * phase_frame_count - 3600) - 1200;
            GameObject.Find("CircleMask").GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelt, sizeDelt);
            if (phase_frame_count == 75)
            {
                transform.Find("Dialogue").gameObject.SetActive(false);
                GameObject.Find("Cameras").transform.Find("cutscene_cam").gameObject.SetActive(true);
                GameObject.Find("Cameras").transform.Find("game_cam").gameObject.SetActive(false);
                GameObject.Find("UI").transform.Find("GameUI").gameObject.SetActive(false);
                Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();
                audio_manager.Stop("bass");
                audio_manager.Stop("drums");
                audio_manager.Stop("epiano");
                audio_manager.Stop("extra_perc");
                audio_manager.Stop("synth");
                audio_manager.Stop("regpiano");
                audio_manager.Stop("choir");
                audio_manager.Play("gym_sound", 1, true);
                DepthOfField tmp;
                if (volume.profile.TryGet<DepthOfField>(out tmp))
                {
                    tmp.active = false;
                }
                GameObject.Find("Players").transform.Find("enemy_left").gameObject.SetActive(true);
                GameObject.Find("Players").transform.Find("hubert").gameObject.SetActive(true);
                GameObject.Find("Players").transform.Find("player").gameObject.SetActive(false);
                GameObject.Find("Players").transform.Find("hubert").gameObject.SetActive(false);
            }
            if (phase_frame_count > 150)
            {
                // done!
                GameObject next_UI_new = Instantiate(next_UI);
                next_UI_new.transform.SetParent(transform.parent);
                next_UI_new.transform.position = new Vector3(960, 540, 0);
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
