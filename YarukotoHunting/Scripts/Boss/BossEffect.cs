using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class BossEffect : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerVCam;
    [SerializeField] private CinemachineVirtualCamera bossVCam;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BossStatus bossStatus;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private bool isPlay = true;
    private int basePriority = 10;
    private bool isStart = false;
    public bool IsStart { get => isStart; }

    private void Start()
    {
        if (!isPlay) return;
        FirstCamerawork();
    }

    public void CameraworkToPlayer()
    {
        bossVCam.Priority = basePriority - 1;
        // プレイヤー操作不可能
        playerController.enabled = true;
    }

    public void CameraworkToBoss()
    {
        bossVCam.Priority = basePriority + 1;
        // プレイヤー操作可能
        playerController.enabled = false;
    }

    private void FirstCamerawork()
    {
        CameraworkToBoss();
        DOVirtual.DelayedCall(5f,() =>
        {
            CameraworkToPlayer();
            DOVirtual.DelayedCall(2f,() =>
            {
                bossStatus.FirstMagicAttack();
                playerCollider.enabled = true;
            });
        });
    }

    public void ParalysisCamerawork()
    {
        CameraworkToBoss();
        DOVirtual.DelayedCall(3f, () =>
        {
            CameraworkToPlayer();
        });
    }

    public void InterpolationOn()
    {
        CinemachineBlendDefinition newBlend = new CinemachineBlendDefinition();
        newBlend.m_Style = CinemachineBlendDefinition.Style.Linear;
        brain.m_DefaultBlend = newBlend;
    }
}

