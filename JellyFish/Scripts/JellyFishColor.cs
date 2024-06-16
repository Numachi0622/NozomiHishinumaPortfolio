using System.Collections;
using UnityEngine;

public class JellyFishColor : MonoBehaviour
{
    [SerializeField] private Gradient defaultGradient; // ��{�̐F�O���f�[�V����
    [SerializeField] private Color lightUpTimeColor; // ���C�g�A�b�v�̐F
    private Color[] defaultColor; // 1���q���Ƃ̐F���
    [SerializeField] private ParticleSystem particle;
    [SerializeField, Range(0f, 1f)] private float lightValue; // ���C�g�A�b�v�̒l��0-1�Ő���
    private float intensityValue = 3f; // ���̋���
    private ParticleSystem.Particle[] particles; //1���q���Ƃ̔z��
    private bool isLightingUp = false; // ���C�g�A�b�v����
    public bool IsLightingUp => isLightingUp; // ���J�p

    private WaitForSeconds wait; // ���C�g�A�b�v���̒��Ԓn�_�ł̑ҋ@����

    private void Start(){
        StartCoroutine(Init());
        wait = new WaitForSeconds(1.5f);
    }

    // �f�t�H���g�̐F�ɏ�����
    IEnumerator Init()
    {
        yield return null;
        particles = new ParticleSystem.Particle[particle.particleCount];
        defaultColor = new Color[particle.particleCount];
        int particleNum = particle.GetParticles(particles);
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].startColor = defaultGradient.Evaluate(Random.Range(0.0f, 1.0f));
            defaultColor[i] = particles[i].startColor;
        }
        particle.SetParticles(particles, particleNum);
    }

    // ���C�g�A�b�v���̐F�ƌ��̋����̐���
    public IEnumerator ColorLerp()
    {
        isLightingUp = true;

        bool isDeclease = false;
        var light = particle.lights;
        while(true){
            yield return null;
            if(lightValue >= 1.0f)
            {
                isDeclease = true;
                yield return wait;
            }

            // �l�������Ă���̂������Ă���̂��ɂ���ĉ��Z�ʂ𐧌�
            float incleaseValue = isDeclease ? -Time.deltaTime : Time.deltaTime;
            lightValue += incleaseValue;
            light.intensityMultiplier += incleaseValue * intensityValue;
            int particleNum = particle.GetParticles(particles);
            for (int i = 0; i < particles.Length; i++)
            {
                // �f�t�H���g��Ԃ̐F�ƃ��C�g�A�b�v�̐F��lightValue�̒l�ɂ���ĕ��
                particles[i].startColor = Color.Lerp(defaultColor[i], lightUpTimeColor, lightValue);
            }
            particle.SetParticles(particles, particleNum);
            if(lightValue < 0)
            {
                isLightingUp = false;
                yield break;
            }
        }
    }
}
