using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightUpManager : MonoBehaviour
{
    [SerializeField] private LightUpCollisionDetector lightUpCollisionDetector;
    [SerializeField] private float lightUppablefingerDistance = 0.01f; // �l�����w�Ɛe�w�̋����i���̒l�ȉ��Ń��C�g�A�b�v�̃g���K�[�ƂȂ�j
    [SerializeField] private float lightUpTime = 3f; // ���C�g�A�b�v�̑S�̂̎���
    private List<JellyFishColor> jellyFishColors = new List<JellyFishColor>(); // �N���Q�̃��X�g
    private Vector3 startPos, endPos; // �n�_, �I�_
    private float widenDistanceValue = 5f; // �n�_�ƏI�_�̋���
    [SerializeField] private Transform indexTf, thumbTf; // �l�����w�Ɛe�w��Transform
    [SerializeField] private GameObject fingerEffectPrefab; // �w�ɒǏ]����G�t�F�N�g�̃v���t�@�u
    private GameObject fingerEffect; // �w�ɒǏ]����G�t�F�N�g���i�[
    private WaitForSeconds interval; // ���Ƀ��C�g�A�b�v����ۂ̃N���Q���̃C���^�[�o��
    [SerializeField] private Collider handCollider; // ��̃R���C�_�[

    private void Start()
    {
        startPos = endPos = Vector3.zero;
        interval = new WaitForSeconds(0.05f);
    }

    private void Update()
    {
        // �l�����w�Ɛe�w�̋������v�Z
        float dist = Vector3.Distance(indexTf.position, thumbTf.position);
        if (dist < lightUppablefingerDistance)
        {
            if (fingerEffect == null)
            {
                fingerEffect = Instantiate(fingerEffectPrefab, indexTf.position, Quaternion.identity, indexTf);
                fingerEffect.GetComponent<ParticleSystem>().Play();
            }

            if (startPos == Vector3.zero)
            {
                // �ŏ��Ɏn�_��ݒ�
                startPos = indexTf.position;
            }
            else
            {
                // �w�𗣂��܂ŏI�_���v�Z����
                endPos = indexTf.position;
            }

            if (handCollider.enabled)
            {
                handCollider.enabled = false;
            }
        }
        else if (dist >= lightUppablefingerDistance && startPos != Vector3.zero)
        {
            // �w�𗣂����u�ԂɎn�_�ƏI�_�̍��W����������v�Z����
            Vector3 direction = (endPos - startPos).normalized;
            // �n�_�ƏI�_�̋������Z�����߁A�������L����
            startPos -= direction * widenDistanceValue;
            endPos += direction * widenDistanceValue;
            // �n�_�ƏI�_��n���A���̊Ԃ��R���C�_�[�������A���C�g�A�b�v���鏇�����肷��
            lightUpCollisionDetector.CheckJellyFishOrder(startPos, endPos);

            startPos = endPos = Vector3.zero;
            if (fingerEffect != null)
            {
                // �w�ɒǏ]���Ă����G�t�F�N�g���q����O���A�w�肵�������ɔ�΂�
                fingerEffect.transform.SetParent(null);
                fingerEffect.GetComponent<FingerEffectAutomaticMove>().SetDirection(direction);
                Destroy(fingerEffect,3);
            }

            handCollider.enabled = true;
        }
    }

    // List�̏��ԂŃ��C�g�A�b�v�����s����
    public IEnumerator JellyFishLightUp()
    {
        var lightUpJellyFish = new List<JellyFishColor>(jellyFishColors);
        // �ǂ����̃N���Q�������Ă�������s���Ȃ�
        if (lightUpJellyFish.Any(jf => jf.IsLightingUp)) yield break;

        foreach(var jellyFish in lightUpJellyFish) 
        {
            StartCoroutine(jellyFish.ColorLerp());
            yield return interval;
        }

        // ���C�g�A�b�v�I������List�̏����N���A
        jellyFishColors.Clear();
    }

    // �O������N���Q��List�ɒl��������
    public void AddJellyFish(JellyFishColor jf)
    {
        jellyFishColors.Add(jf);
    }
}
