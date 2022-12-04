using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class menu_controllable : MonoBehaviour
{
    public virtual void MoveByVector(Vector2 dir)
    {
        float angle = Vector2.Angle(new Vector2(1, 0), dir);
        if (dir.y < 0) angle = 360 - angle;
        MoveByAngle(angle);
    }

    public virtual void MoveByAngle(float angle)
    {
        print("Move Highlight not implemented");
    }

    public virtual menu_controllable A_Pressed()
    {
        print("A_Pressed not implemented");
        return null;
    }

    public virtual void Start_Pressed()
    {
        print("Start_Pressed not implemented");
    }

    public GameObject create_prefab(string name)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        int start_index = name.LastIndexOf('/') + 1;
        newfab.name = name.Substring(start_index, name.Length - start_index);
        return newfab;
    }

    public menu_controllable load_game(string left_player, string right_player, bool real_game, int enemy_level)
    {
        // load and start a game

        GameObject gym = create_prefab("Gym");
        gym.transform.position = Vector3.zero;
        gym.transform.Find("establishcam_pivot").gameObject.SetActive(false);

        GameObject game = create_prefab("Game");
        game.transform.position = Vector3.zero;
        game.GetComponent<game_manager>().set_real_game(real_game);
        game.transform.Find("game_cam").gameObject.SetActive(true);
        game.transform.Find("Players").Find(right_player).gameObject.SetActive(true);
        game.transform.Find("Players").Find(left_player).gameObject.SetActive(true);
        game.GetComponent<game_manager>().set_players(game.transform.Find("Players").Find(left_player), game.transform.Find("Players").Find(right_player));
        if (real_game) game.transform.Find("Players").Find(left_player).GetComponent<marvin_behavior>().begin_serve();
        if (right_player.Equals("enemy_right"))
            game.transform.Find("Players").Find(right_player).GetComponent<enemy_control>().set_level(enemy_level);

        transform.parent.parent.Find("GameUI").gameObject.SetActive(true);

        // enable pausing
        GameObject.Find("UI").GetComponent<Canvas>().worldCamera = game.transform.Find("game_cam").GetComponent<Camera>();
        GameObject.Find("UI").GetComponent<Canvas>().planeDistance = 0.5f;

        return GameObject.Find("UI").transform.Find("Menu").Find("Pause").GetComponent<menu_pause>();
    }
}
