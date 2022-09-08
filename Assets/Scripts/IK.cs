using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    public GameObject start;
    public GameObject upper; // a
    public GameObject lower; // b
    public GameObject end;
    public GameObject target;
    public GameObject pole;

    void Update()
    {
        start.transform.LookAt(target.transform.position, pole.transform.position - start.transform.position);

        float a = Vector3.Distance(upper.transform.position, lower.transform.position);
        float b = Vector3.Distance(lower.transform.position, end.transform.position);
        float c = Vector3.Distance(upper.transform.position, target.transform.position);

        float upper_angle = -LawOfCosines(a, c, b) * Mathf.Rad2Deg;
        float lower_angle = -180 - LawOfCosines(b, a, c) * Mathf.Rad2Deg;

        if (float.IsNaN(upper_angle))
        {
            upper_angle = 0;
            lower_angle = 0;
        }

        upper.transform.rotation = new Quaternion();
        upper.transform.RotateAround(upper.transform.position, upper.transform.right, upper_angle);

        lower.transform.rotation = new Quaternion();
        lower.transform.RotateAround(lower.transform.position, lower.transform.right, lower_angle);
    }

    float LawOfCosines(float a, float b, float c)
    {
        return Mathf.Acos((a * a + b * b - c * c) / (2 * a * b));
    }
}
