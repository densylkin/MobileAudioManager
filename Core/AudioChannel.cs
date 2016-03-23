using System;
using UnityEngine;
using System.Collections;

public class AudioChannel : MonoBehaviour
{
    public bool UseFading;

    public float FadeInDuration = 0.5f;
    public float FadeOutDuration = 0.5f;

    public event Action<AudioChannel> FinishedPlaying;

    [SerializeField, HideInInspector]
    public AudioSource Source;

    /// <summary>
    /// Set channel`s AudioClip
    /// </summary>
    public AudioClip Clip
    {
        get { return Source.clip; }
        set { Source.clip = value; }
    }

    /// <summary>
    /// Get play position
    /// </summary>
    public float Progress
    {
        get { return Clip != null ? Source.timeSamples/Clip.samples : 0f; }
    }

    public float ClipLength
    {
        get { return Source.clip.length; }
    }

    public float PlayTime
    {
        get { return Source.time; }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool Finished
    {
        get { return Progress >= 1f; }
    }

    /// <summary>
    /// Mute or unmute channel
    /// </summary>
    public bool Mute
    {
        get { return Source.mute; }
        set { Source.mute = value; }
    }

    /// <summary>
    /// Play / Pause channel  
    /// </summary>
    public bool Playing
    {
        get { return Source.isPlaying; }
        set
        {
            if(value)
                Play();
            else
                Pause();
        }
    }

    private void Update()
    {
        if(Finished && Playing)
            OnFinished(this);
    }

    /// <summary>
    /// Create AudioChannel
    /// </summary>
    /// <param name="fadeInDuration"></param>
    /// <param name="fadeOutDuration"></param>
    /// <returns></returns>
    public static AudioChannel Create(float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f)
    {
        var go = new GameObject("AudioChannel");
        var channel = go.AddComponent<AudioChannel>();
        channel.Source = go.AddComponent<AudioSource>();

        return channel;
    }

    /// <summary>
    /// Play Audioclip
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioClip clip)
    {
        Clip = clip;
        Play();
    }

    /// <summary>
    /// Play current Clip
    /// </summary>
    public void Play()
    {
        if (Clip != null)
        {
            if (UseFading)
            {
                Source.Play();
                //FadeOut();
            }
            else
                Source.Play();
        }
        else
            Debug.Log("No audioclip set");
    }

    /// <summary>
    /// Pause 
    /// </summary>
    public void Pause()
    {
        if(Playing)
            Source.Pause();
        else
            Debug.Log("Source is already paused or stopped");
    }

    /// <summary>
    /// Stop playing
    /// </summary>
    public void Stop()
    {
        if (Playing)
            Source.Stop();
        else
            Debug.Log("Source is already paused or stopped");
    }

    /// <summary>
    /// 
    /// </summary>
    [ContextMenu("FadeIn")]
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeTo(Source.volume, 0f, FadeInDuration));
    }

    /// <summary>
    /// 
    /// </summary>
    [ContextMenu("FadeOut")]
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeTo(Source.volume, 1f, FadeOutDuration));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void OnFinished(AudioChannel obj)
    {
        var handler = FinishedPlaying;
        if (handler != null) 
            handler(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IEnumerator FadeTo(float from, float to, float duration)
    {
        var startTime = Time.time;
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            var normalizedtime = Mathf.Clamp01(elapsedTime/duration);
            Source.volume = Mathf.Lerp(from, to, normalizedtime);
            yield return null;
        }
    }
}
