using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor; 
#endif

[RequireComponent(typeof(AudioListener))]
[AddComponentMenu("Audio/Manager")]
public class AudioManager : MonoBehaviour
{
    public enum Channel
    {
        Music = 0,
        First = 1,
        Second = 2, 
        Third = 3
    }

    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<AudioManager>();
            if (_instance == null)
            {
                _instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                _instance.gameObject.AddComponent<MusicManager>();
                _instance.CreateChannels();
            }
            return _instance;
        }
    }

    [SerializeField, HideInInspector]
    private AudioChannel[] _channels;
    private readonly  Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

    public static bool MusicEnabled
    {
        get { return Instance._channels != null && !Instance._channels[0].Mute; }
        set { Instance._channels[0].Mute = !value; }
    }

    public static bool SfxEnabled
    {
        get { return Instance._channels != null && !Instance._channels[1].Mute; }
        set
        {
            for(var i = 1; i < 4; i++)
                Instance._channels[i].Mute = !value;
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Create Other/AudioManager")]
    public static void Create()
    {
        if (Instance != null)
        {
            Debug.Log("AudioManager already exists!");
            return;
        }

        var go = new GameObject("AudioManager");
        _instance = go.AddComponent<AudioManager>();
        go.AddComponent<MusicManager>();

        Instance.CreateChannels();
    } 
#endif

    private void Awake()
    {
        CreateChannels();
    }

    private void CreateChannels()
    {
        if (_channels != null)
            return;

        _channels = new AudioChannel[4];
        for (var i = 0; i < 4; i++)
        {
            var c = AudioChannel.Create();
            c.gameObject.name += i == 0 ? " Music" : " Sfx " + i;
            c.transform.SetParent(transform);
            c.transform.localPosition = Vector3.zero;
            _channels[i] = c;
        }
    }

    public static AudioChannel GetChannel(Channel channel)
    {
        return Instance._channels[(int) channel];
    }

    public static void Play(string clipName, Channel channel = Channel.First, float pitch = 1f, float volume = 1f)
    {
        Play(Load(clipName), channel, pitch, volume);
    }

    public static void Play(AudioClip clip, Channel channel = Channel.First, float pitch = 1f, float volume = 1f)
    {
        if (clip == null)
            return;

        var c = Instance._channels[(int) channel];
        c.Source.pitch = pitch;
        c.Source.volume = volume;
        c.Play(clip);
    }

    public static void Pause(Channel channel)
    {
        Instance._channels[(int) channel].Pause();
    }

    public void Stop(Channel channel)
    {
        Instance._channels[(int) channel].Stop();
    }

    public static AudioClip Load(string clipName)
    {
        return Instance._clips.ContainsKey(clipName) ? Instance._clips[clipName] : Resources.Load<AudioClip>(clipName);
    }
}
