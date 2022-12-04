using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helpful_hubert : MonoBehaviour
{
    GameObject shuttle;
    audio_manager audio_manager;

    void Start()
    {
        audio_manager = GameObject.Find("audio_manager").GetComponent<audio_manager>();
        shuttle = GameObject.Find("shuttle");
    }

    public void hit_shuttle(Vector3 where)
    {
        GameObject new_shuttle = create_prefab("shuttle");
        new_shuttle.transform.parent = GameObject.Find("Game").transform.Find("shuttles");
        new_shuttle.transform.localPosition = Vector3.zero;
        new_shuttle.transform.rotation = Quaternion.identity;

        new_shuttle.GetComponent<shuttle_behavior>().set_towards_right(false);
        new_shuttle.GetComponent<shuttle_behavior>().set_trajectory(
            transform.localPosition + Vector3.up,
            where,
            15,
            false);
        audio_manager.Play("hit soft", 1);
        transform.Find("hubert_model").GetComponent<Animator>().SetTrigger("serve");
    }

    GameObject create_prefab(string name)
    {
        GameObject newfab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        int start_index = name.LastIndexOf('/') + 1;
        newfab.name = name.Substring(start_index, name.Length - start_index);
        return newfab;
    }
}
