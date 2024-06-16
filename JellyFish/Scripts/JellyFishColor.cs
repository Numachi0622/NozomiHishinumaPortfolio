using System.Collections;
using UnityEngine;

public class JellyFishColor : MonoBehaviour
{
    [SerializeField] private Gradient defaultGradient; // 基本の色グラデーション
    [SerializeField] private Color lightUpTimeColor; // ライトアップの色
    private Color[] defaultColor; // 1粒子ごとの色情報
    [SerializeField] private ParticleSystem particle;
    [SerializeField, Range(0f, 1f)] private float lightValue; // ライトアップの値を0-1で制御
    private float intensityValue = 3f; // 光の強さ
    private ParticleSystem.Particle[] particles; //1粒子ごとの配列
    private bool isLightingUp = false; // ライトアップ中か
    public bool IsLightingUp => isLightingUp; // 公開用

    private WaitForSeconds wait; // ライトアップ中の中間地点での待機時間

    private void Start(){
        StartCoroutine(Init());
        wait = new WaitForSeconds(1.5f);
    }

    // デフォルトの色に初期化
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

    // ライトアップ中の色と光の強さの制御
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

            // 値が増えているのか減っているのかによって加算量を制御
            float incleaseValue = isDeclease ? -Time.deltaTime : Time.deltaTime;
            lightValue += incleaseValue;
            light.intensityMultiplier += incleaseValue * intensityValue;
            int particleNum = particle.GetParticles(particles);
            for (int i = 0; i < particles.Length; i++)
            {
                // デフォルト状態の色とライトアップの色をlightValueの値によって補間
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
