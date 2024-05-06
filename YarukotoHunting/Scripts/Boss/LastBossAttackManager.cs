using UnityEngine;

public class LastBossAttackManager : AttackManager
{
    [SerializeField] private FireBallGenerator fireBallGenerator;
    [SerializeField] private Transform PlayerTf;
    [SerializeField] private LastBossStatus status;
    [SerializeField] private AudioClip fireBallSE;

    public void FireBall()
    {
        audioSource.PlayOneShot(fireBallSE);
        if (status.isAwakening)
        {
            fireBallGenerator.GenerateAwakeningFireBall(PlayerTf);
            return;
        }
        fireBallGenerator.GenerateFireBall();
    }
}
