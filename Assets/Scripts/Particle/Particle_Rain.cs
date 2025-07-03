using System.Collections;
using UnityEngine;

public class Particle_Rain : MonoBehaviour
{
    [Header("������������")]
    [SerializeField] private AnimationCurve intensityCurve;

    [Header("��������")]
    [SerializeField] private float drizzleMinParticle;
    [SerializeField] private float drizzleMaxParticle;
    [SerializeField] private float moderateRainMinParticle;
    [SerializeField] private float moderateRainMaxParticle;
    [SerializeField] private float heavyRainMinParticle;
    [SerializeField] private float heavyRainMaxParticle;

    private float minParticle;
    private float maxParticle;

    [Header("������Ϣ")]
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

            //����׶�
            yield return new WaitForSeconds(duration);

            float randomValue = Random.value;

            //������
            if (randomValue <= drizzleProbability)
            {
                //�´���
                if (randomValue <= heavyRainProbability)
                {
                    SetMinMaxParticle(heavyRainMinParticle, heavyRainMaxParticle);
                }
                //������
                else if (randomValue <= moderateRainProbability)
                {
                    SetMinMaxParticle(moderateRainMinParticle, moderateRainMaxParticle);
                }
                else
                {
                    SetMinMaxParticle(drizzleMinParticle, drizzleMaxParticle);
                }

                //��ʼ����
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
    /// ������̣����꿪ʼ�������������
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
    /// ������������
    /// </summary>
    /// <param name="_min">��С������</param>
    /// <param name="_max">���������</param>
    private void SetMinMaxParticle(float _min, float _max)
    {
        minParticle = _min;
        maxParticle = _max;
    }
}
