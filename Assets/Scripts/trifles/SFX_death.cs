using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_death : MonoBehaviour
{
    void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying) Destroy(gameObject);
    }
}
