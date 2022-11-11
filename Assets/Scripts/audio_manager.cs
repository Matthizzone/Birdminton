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
    }

    public void Play(string clip_name, float volume, bool loop)
    {
        try
        {
            if (SFX[clip_name].isPlaying)
            {
                SFX[clip_name].volume = volume;
            }
            else
            {
                SFX[clip_name].Play();
                SFX[clip_name].loop = loop;
                SFX[clip_name].volume = volume;
            }
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }

    public void Play(string clip_name)
    {
        Play(clip_name, 1, false);
    }

    public void Play(string clip_name, float volume)
    {
        Play(clip_name, volume, false);
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
