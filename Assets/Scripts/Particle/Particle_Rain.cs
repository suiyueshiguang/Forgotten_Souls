using System.Collections;
using UnityEngine;

public class Particle_Rain : MonoBehaviour
{
    [Header("雨量曲线配置")]
    [SerializeField] private AnimationCurve intensityCurve;

    [Header("雨量参数")]
    [SerializeField] private float drizzleMinParticle;
    [SerializeField] private float drizzleMaxParticle;
    [SerializeField] private float moderateRainMinParticle;
    [SerializeField] private float moderateRainMaxParticle;
    [SerializeField] private float heavyRainMinParticle;
    [SerializeField] private float heavyRainMaxParticle;

    private float minParticle;
    private float maxParticle;

    [Header("天气信息")]
    [SerializeField] private float dryDuration;
    [SerializeField] private float rainDuration;
    [Range(0f, 1f)]
    [SerializeField] private float drizzleProbability;
    [Range(0f, 1f)]
    [SerializeField] private float moderateRainProbability;
    [Range(0f, 1f)]
    [SerializeField] private float heavyRainProbability;

    private ParticleSystem rainParticleSystem;
    private ParticleSystem.EmissionModule emissionModule;
    private Coroutine rainingCoroutine;

    private void Start()
    {
        rainParticleSystem = GetComponent<ParticleSystem>();
        emissionModule = rainParticleSystem.emission;
        emissionModule.rateOverTime = 0;

        StartCoroutine(WeatherCycle());
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            float duration = dryDuration * Random.Range(0.8f, 1.2f);

            //晴天阶段
            yield return new WaitForSeconds(duration);

            float randomValue = Random.value;

            //下雨检查
            if (randomValue <= drizzleProbability)
            {
                //下大雨
                if (randomValue <= heavyRainProbability)
                {
                    SetMinMaxParticle(heavyRainMinParticle, heavyRainMaxParticle);
                }
                //下中雨
                else if (randomValue <= moderateRainProbability)
                {
                    SetMinMaxParticle(moderateRainMinParticle, moderateRainMaxParticle);
                }
                else
                {
                    SetMinMaxParticle(drizzleMinParticle, drizzleMaxParticle);
                }

                //开始下雨
                if (rainingCoroutine != null)
                {
                    StopCoroutine(rainingCoroutine);
                }

                rainingCoroutine = StartCoroutine(RainProcess());

                yield return rainingCoroutine;
            }
        }
    }

    /// <summary>
    /// 下雨过程（下雨开始——下雨结束）
    /// </summary>
    private IEnumerator RainProcess()
    {
        rainParticleSystem.Play();

        float timer = 0f;
        float duration = rainDuration * Random.Range(0.8f, 1.2f);

        while (timer < duration)
        {
            float curveValue = intensityCurve.Evaluate(timer / duration);
            float currentRate = Mathf.Lerp(minParticle, maxParticle, curveValue);

            emissionModule.rateOverTime = currentRate;

            timer += Time.deltaTime;
            yield return null;
        }

        rainParticleSystem.Stop();
    }

    /// <summary>
    /// 设置粒子数量
    /// </summary>
    /// <param name="_min">最小粒子数</param>
    /// <param name="_max">最大粒子数</param>
    private void SetMinMaxParticle(float _min, float _max)
    {
        minParticle = _min;
        maxParticle = _max;
    }
}
