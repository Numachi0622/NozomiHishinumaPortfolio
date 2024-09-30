using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundData")]
public class SoundData : ScriptableObject
{
    [Header("BGM")] 
    public AudioClip Title;
    public AudioClip Battle;
    public AudioClip Result;

    [Header("SE")] 
    public AudioClip Impact;
    public AudioClip Fire;
    public AudioClip ComicalEffectStart;
    public AudioClip TextSending;
    public AudioClip BlowAway;
    public AudioClip Hit;
    public AudioClip PhaseStart;
    public AudioClip Generate;
    public AudioClip Destroy;
    public AudioClip MultipleHit;
    public AudioClip Score;
    public AudioClip ScoreList;
    public AudioClip Finish;
    public AudioClip CountDown;

    public Dictionary<string, AudioClip> BGMData, SEData;

    public void Init()
    {
        BGMData = new Dictionary<string, AudioClip>()
        {
            {"Title", Title},
            {"Battle", Battle },
            {"Result", Result }
        };
        SEData = new Dictionary<string, AudioClip>()
        {
            {"Fire", Fire },
            {"Impact", Impact },
            {"ComicalEffectStart",ComicalEffectStart },
            {"TextSending", TextSending },
            {"BlowAway",BlowAway },
            {"Hit", Hit },
            {"PhaseStart", PhaseStart },
            {"Generate", Generate },
            {"Destroy", Destroy },
            {"MultipleHit", MultipleHit },
            {"ScoreList", ScoreList },
            {"Score",Score },
            {"Finish", Finish },
            {"CountDown", CountDown }
        };
    }
}
