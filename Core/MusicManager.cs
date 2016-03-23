using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityHelpers;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance { get { return _instance ?? (_instance = FindObjectOfType<MusicManager>());} }

    public bool Shuffle;
    public float FadeDuration = 3f;
    public List<AudioClip> Tracks = new List<AudioClip>();

    public int TracksCount { get { return Tracks.Count; } }
    public AudioClip CurrentTrack { get { return _currenTrack == -1 ? null : Tracks[_currenTrack]; } }

    private AudioChannel _musicChannel;
    private int _currenTrack = -1;

    private void Start()
    {
        _musicChannel = AudioManager.GetChannel(AudioManager.Channel.Music);
        _musicChannel.FinishedPlaying += FinishedCurrent;
        StartCoroutine(PlayList());
    }

    private void Update()
    {
        
    }

    public static void Play()
    {
        Instance._musicChannel.Clip = Instance.CurrentTrack;
        Instance._musicChannel.Play();
    }

    public static void Pause()
    {
        Instance._musicChannel.Pause();
    }

    public static void Next()
    {
        Instance.ChangeTrack(true);
    }

    public static void Previous()
    {
        Instance.ChangeTrack(false);
    }

    public void ChangeTrack(bool direction)
    {
        if (Shuffle)
            SetRandomTrack();
        else
        {
            if (direction)
            {
                if (_currenTrack == TracksCount - 1)
                    _currenTrack = 0;
                else
                    _currenTrack++;
            }
            else
            {
                if (_currenTrack == 0)
                    _currenTrack = TracksCount - 1;
                else
                    _currenTrack--;
            }
        }
    }

    private IEnumerator PlayList()
    {
        while (true)
        {
            yield return StartCoroutine(PlayTrack(CurrentTrack));
            ChangeTrack(true);
        }
    }

    private IEnumerator PlayTrack(AudioClip clip)
    {
        _musicChannel.Play(clip);
        StartCoroutine(_musicChannel.FadeIn());
        while (_musicChannel.Playing)
        {
            if (_musicChannel.ClipLength - _musicChannel.PlayTime <= FadeDuration)
            {
                yield return StartCoroutine(_musicChannel.FadeOut());
            }
            yield return null;
        }
    }

    //private IEnumerator PlaySongE(AudioClip clip)
    //{
    //    source.Stop();
    //    source.clip = clip;
    //    source.Play();
    //    StartCoroutine(FadeIn());
    //    while (source.isPlaying)
    //    {
    //        if (source.clip.length - source.time <= FadeDuration)
    //        {
    //            yield return StartCoroutine(FadeOut());
    //        }
    //        yield return null;
    //    }
    //}

    private int _counter = 0;

    //private IEnumerator PlayMusicList()
    //{
    //    while (true)
    //    {
    //        yield return StartCoroutine(PlaySongE(Playlist.MusicList[_counter]));
    //        if (Repeat == RepeatMode.Track)
    //        {

    //        }
    //        else if (Shuffle)
    //        {
    //            int newTrack = GetNewTrack();
    //            while (newTrack == _counter && Playlist.MusicList.Length != 1)
    //            {
    //                newTrack = GetNewTrack();
    //            }
    //            _counter = newTrack;

    //        }
    //        else
    //        {
    //            _counter++;
    //            if (_counter >= Playlist.MusicList.Length - 1)
    //            {
    //                if (Repeat == RepeatMode.Playlist)
    //                {
    //                    _counter = 0;
    //                }
    //                else
    //                    yield break;
    //            }
    //        }
    //    }
    //}

    private void SetRandomTrack()
    {
        _currenTrack = RandomHelpers.GetRandomInt(TracksCount);
    }

    private void FinishedCurrent(AudioChannel channel)
    {
        
    }
}
