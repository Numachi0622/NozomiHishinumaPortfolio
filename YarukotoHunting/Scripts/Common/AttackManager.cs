using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private WaitForSeconds delayTime = new WaitForSeconds(0.2f);

    [SerializeField] private Collider attackCollider; // 攻撃用のコライダー
    [SerializeField] private GameObject slashParticle; // 斬撃エフェクト

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] private AudioClip attackSE;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GameObject.FindGameObjectWithTag("EnemyAttackSE").GetComponent<AudioSource>();
    }

    // 攻撃開始時に呼ばれる関数(敵用)
    public void OnAttackStart()
    {
        attackCollider.enabled = true;
        audioSource.PlayOneShot(attackSE);
        // 斬撃エフェクトはプレイヤーのみ
        if(slashParticle != null) slashParticle.SetActive(true);
        StartCoroutine(AttackFinishDelayCoroutine());
    }

    // 攻撃終了時に呼ばれる関数
    public void OnAttackFinished()
    {
        attackCollider.enabled = false;
        // 斬撃エフェクトはプレイヤーのみ
        if (slashParticle != null) slashParticle.SetActive(false);
    }

    IEnumerator AttackFinishDelayCoroutine()
    {
        yield return delayTime;
        OnAttackFinished();
    }
}
