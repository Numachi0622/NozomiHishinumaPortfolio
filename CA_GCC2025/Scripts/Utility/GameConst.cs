using UnityEngine;

namespace Utility
{
    public class GameConst
    {
        public static float MIN_MOVABLE_AREA = -25f;
        public static float MAX_MOVABLE_AREA = 25f;
        public static float ANIM_FPS = 30f;
        public static float COLLIDER_ACTIVE_TIME = 0.02f;
        public static int MAX_SKILL_POINT = 3;
        public static int SKILL_DECREASE_VALUE = 1;
        public static int SP_SKILL_DECREASE_VALUE = 3;
    }

    [System.Serializable]
    public struct BasePower
    {
        [SerializeField] private int MinPower;
        [SerializeField] private int MaxPower;
        public int GetRandomPower => Random.Range(MinPower, MaxPower + 1);
    }

    [System.Serializable]
    public struct CriticalMagnification
    {
        [SerializeField] private int MinMag;
        [SerializeField] private int MaxMag;
        public int GetRandomMagnigication => Random.Range(MinMag, MaxMag + 1);
    }
}