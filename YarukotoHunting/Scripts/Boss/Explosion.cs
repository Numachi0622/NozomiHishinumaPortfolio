using UnityEngine;
using DG.Tweening;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private AudioSource explosionSource;
    [SerializeField] private AudioClip explosionSE;

    public void ExplosionEvent()
    {
        explosionParticle.SetActive(true);
        explosionSource.PlayOneShot(explosionSE);
        DOVirtual.DelayedCall(1.5f, () => explosionParticle.SetActive(false));
    }
}
