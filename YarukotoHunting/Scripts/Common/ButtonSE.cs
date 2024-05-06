using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonSE;

    public void PlayButtonSE()
    {
        audioSource.PlayOneShot(buttonSE);
    }
}
