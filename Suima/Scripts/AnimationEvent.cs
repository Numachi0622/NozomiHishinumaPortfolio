using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent[] animEvent;
    private int eventCount = 0;

    public void AnimEvent()
    {
        animEvent[eventCount]?.Invoke();
        eventCount++;
    }

    public void SoundEvent(string seKey)
    {
        SoundManager.Instance.PlaySE(seKey);
    }
}
