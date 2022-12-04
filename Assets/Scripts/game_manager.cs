using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour
{
    // Global References

    Transform game_ui;

    // Internal System

    int left_score = 0;
    int right_score = 0;
    float shuttle_death = Mathf.Infinity;
    bool transitions = false; // in practice mode, score only, no transitions
    bool mid_transition = false;

    // Parameters

    float dead_time = 1.5f; // time in between shuttle death and animation start


    private void Start()
    {
        game_ui = GameObject.Find("UI").transform.Find("GameUI");
    }

    void Update()
    {
        if (transitions)
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
        }
    }

    public void ShuttleDied(bool winner_side)
    {
        shuttle_death = Time.time;

        if (winner_side) left_score++;
        else right_score++;
        game_ui.Find("Score").Find("Text").GetComponent<TMPro.TMP_Text>().text = left_score + " - " + right_score;
    }

    public void set_transitions(bool new_transitions)
    {
        transitions = new_transitions;
    }
}
