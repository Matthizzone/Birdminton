using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_manager : MonoBehaviour
{
    IDictionary<string, AudioSource> SFX = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        AudioClip[] SFX_list = Resources.LoadAll<AudioClip>("SFX");

        foreach (var sfx in SFX_list)
        {
            SFX[sfx.name] = gameObject.AddComponent<AudioSource>();
            SFX[sfx.name].clip = sfx;
            SFX[sfx.name].playOnAwake = false;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void Play(string clip_name)
    {
        try
        {
            SFX[clip_name].volume = 1;
            SFX[clip_name].Play();
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }

    public void Play(string clip_name, float volume)
    {
        try
        {
            SFX[clip_name].volume = volume;
            SFX[clip_name].Play();
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }

    public void Stop(string clip_name)
    {
        try
        {
            SFX[clip_name].Stop();
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }
}
