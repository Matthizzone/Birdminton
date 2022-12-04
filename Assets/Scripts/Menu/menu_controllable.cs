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
}
