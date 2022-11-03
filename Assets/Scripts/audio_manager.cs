using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_manager : MonoBehaviour
{
    IDictionary<string, AudioSource> SFX = new Dictionary<string, AudioSource>();
    IDictionary<string, AudioSource> Music = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        AudioClip[] SFX_list = Resources.LoadAll<AudioClip>("SFX");

        foreach (var sfx in SFX_list)
        {
            SFX[sfx.name] = gameObject.AddComponent<AudioSource>();
            SFX[sfx.name].clip = sfx;
            SFX[sfx.name].playOnAwake = false;
        }

        AudioClip[] Music_list = Resources.LoadAll<AudioClip>("Music");

        foreach (var mus in Music_list)
        {
            Music[mus.name] = gameObject.AddComponent<AudioSource>();
            Music[mus.name].clip = mus;
            Music[mus.name].playOnAwake = false;
            Music[mus.name].loop = true;
        }
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

    public void Loop(string clip_name, float volume)
    {
        try
        {
            Music[clip_name].volume = volume;
            Music[clip_name].Play();
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }
}
