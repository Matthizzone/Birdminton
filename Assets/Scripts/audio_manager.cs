using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_manager : MonoBehaviour
{
    IDictionary<string, AudioSource> Sounds = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        AudioClip[] SFX_list = Resources.LoadAll<AudioClip>("Sounds");

        foreach (var sfx in SFX_list)
        {
            Sounds[sfx.name] = gameObject.AddComponent<AudioSource>();
            Sounds[sfx.name].clip = sfx;
            Sounds[sfx.name].playOnAwake = false;
        }
    }

    public void Play(string clip_name, float volume, bool loop)
    {
        try
        {
            if (Sounds[clip_name].isPlaying)
            {
                Sounds[clip_name].volume = volume;
            }
            else
            {
                Sounds[clip_name].Play();
                Sounds[clip_name].loop = loop;
                Sounds[clip_name].volume = volume;
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
            Sounds[clip_name].Stop();
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }

    public void Play_SFX(string clip_name)
    {
        try
        {
            Sounds[clip_name].Play();
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
        }
    }

    public void PlayMany(string clip_name)
    {
        AudioSource AS;

        try
        {
            AS = Sounds[clip_name];
        }
        catch (KeyNotFoundException)
        {
            print("clip not found: " + clip_name);
            return;
        }

        GameObject mini_speaker = new GameObject();
        mini_speaker.AddComponent<AudioSource>();
        mini_speaker.GetComponent<AudioSource>().clip = AS.clip;
        mini_speaker.GetComponent<AudioSource>().Play();
        mini_speaker.name = clip_name;
        mini_speaker.AddComponent<SFX_death>();
        mini_speaker.transform.parent = transform;
    }
}
