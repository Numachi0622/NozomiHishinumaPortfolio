using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float defaultDistance = 3.24f; // デフォルトのプレイヤーとカメラの距離
    [SerializeField] private float wallHitDistance = 0.9f; // 壁付近にいるときのプレイヤーとカメラの距離
    private float eventDistance = 2.2f;
    private bool updateStopping;
    private CinemachineVirtualCamera vCam;
    private CinemachineFramingTransposer transposer;
    private Transform tf;
    [SerializeField] private Transform playerTf;
    [SerializeField] private LayerMask obstacleLayer;

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        tf = transform;
        updateStopping = false;
    }
    private void Update()
    {
        if (updateStopping) return;
        RaycastHit hit;
        if (Physics.Linecast(tf.position, playerTf.position, out hit, obstacleLayer))
            transposer.m_CameraDistance = wallHitDistance;
        else
            transposer.m_CameraDistance = defaultDistance;
    }
    // イベントが発生し、一時的にカメラのDistance値を変更（Update関数を無視する）
    public void ChangedDistanceByEvent()
    {
        updateStopping = true;
        transposer.m_CameraDistance = eventDistance;
        StartCoroutine(ReturnDistance());
    }
    // 距離を元に戻し、Update関数を再び実行
    IEnumerator ReturnDistance()
    {
        yield return new WaitForSeconds(3);
        updateStopping = false;
    }
}
