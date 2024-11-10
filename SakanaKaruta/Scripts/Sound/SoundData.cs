using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip TurnOverCard;
    public AudioClip ShowCard;
    public AudioClip Hit;
    public AudioClip GetFish;
    public AudioClip AddScore;
    public AudioClip Miss;
    public AudioClip GameFinish;
    public Dictionary<string, AudioClip> Sounds;

    public void Init()
    {
        Sounds = new Dictionary<string, AudioClip>()
        {
            {"TurnOverCard", TurnOverCard },
            {"ShowCard", ShowCard },
            {"Hit", Hit },
            {"GetFish", GetFish },
            {"AddScore", AddScore},
            {"Miss", Miss},
            {"GameFinish", GameFinish }
        };
    }
}
