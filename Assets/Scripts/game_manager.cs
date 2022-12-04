using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour
{
    // Global References

    Transform game_ui;
    audio_manager audio_manager;

    Transform left_player;
    Transform right_player;

    // Internal System

    int left_score = 0;
    int right_score = 0;
    float shuttle_death = Mathf.Infinity;
    bool real_game = false; // in practice mode, score only, no transitions
    bool mid_transition = false;

    // Parameters

    float dead_time = 1.5f; // time in between shuttle death and animation start


    private void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        game_ui = GameObject.Find("UI").transform.Find("GameUI");
    }

    void Update()
    {
        if (real_game)
        {
            if (!mid_transition && Time.time - shuttle_death > dead_time)
            {
                //begin transition
                game_ui.GetComponent<game_ui_transitions>().new_transition();
                mid_transition = true;
            }
            else if (Time.time - shuttle_death > dead_time + 0.5f)
            {
                // make changes while screen is hidden
                foreach (Transform child in GameObject.Find("Game").transform.Find("shuttles"))
                {
                    Destroy(child.gameObject);
                }
                
                GameObject.Find("player").GetComponent<marvin_behavior>().begin_serve();
                shuttle_death = Mathf.Infinity;
                mid_transition = false;
            }

            Transform shuttle = transform.Find("shuttles").Find("shuttle");
            if (shuttle != null)
            {
                // uh
            }
        }
    }

    public void ShuttleDied(bool winner_side)
    {
        shuttle_death = Time.time;

        if (winner_side) left_score++;
        else right_score++;
        game_ui.Find("Score").Find("Text").GetComponent<TMPro.TMP_Text>().text = left_score + " - " + right_score;
    }

    public void set_real_game(bool new_real_game)
    {
        real_game = new_real_game;
    }

    public void close_call()
    {
        if (real_game)
        {
            audio_manager.Play_SFX("crowd gasp");
        }
    }

    public void set_players(Transform new_left, Transform new_right)
    {
        left_player = new_left;
        right_player = new_right;
    }

    public Transform get_left_player()
    {
        return left_player;
    }

    public Transform get_right_player()
    {
        return right_player;
    }
}
