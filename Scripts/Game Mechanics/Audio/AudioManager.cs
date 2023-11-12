using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioManager : MonoBehaviour
{


    [SerializeField] private Sound[] sounds;
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private AudioMixerGroup mixerGroup2;


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //DontDestroyOnLoad(gameObject);
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            if (sound.name.Contains("Background"))
                sound.source.outputAudioMixerGroup = mixerGroup;
            else if (sound.name.Contains("Effect"))
                sound.source.outputAudioMixerGroup = mixerGroup2;
        }
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound was not found");
            return;
        }
        
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }

        s.source.Stop();
    }
    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }
}
