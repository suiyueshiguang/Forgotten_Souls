using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    private new Light2D light;
    private float timeInGame;//��Ϸʱ��ֳ������Σ����峿�����ϣ����磬�������ϣ���ҹ

    [Header("��ʵʱ���е�ÿ���Ӧ��Ϸ���ٷ���")]
    [SerializeField] private float realSecondsPerGameMinute = 1;

    [Header("������ɫ")]
    [SerializeField] private Gradient lightColor;

    private AnimationCurve intensityCurve;

    private void Start()
    {
        light = GetComponent<Light2D>();

        InitializeCurve();
    }

    private void Update()
    {
        //������Ϸʱ��
        timeInGame += Time.deltaTime * realSecondsPerGameMinute / 1440f;

        timeInGame %= 1;

        light.color = lightColor.Evaluate(timeInGame);
        light.intensity = intensityCurve.Evaluate(timeInGame);
    }

    private void InitializeCurve()
    {
        intensityCurve = new AnimationCurve();

        //�峿���
        intensityCurve.AddKey(new Keyframe(0.0f, 0.5f));
        //�峿�յ�
        intensityCurve.AddKey(new Keyframe(0.16f, 1f));
        //����
        intensityCurve.AddKey(new Keyframe(0.33f, 1.5f));
        //����
        intensityCurve.AddKey(new Keyframe(0.5f, 1.5f));
        //����
        intensityCurve.AddKey(new Keyframe(0.66f, 0.8f));
        //����
        intensityCurve.AddKey(new Keyframe(0.83f, 0.3f));
        //��ҹ
        intensityCurve.AddKey(new Keyframe(1.0f, 0.5f));

        intensityCurve.postWrapMode = WrapMode.Loop;
    }
}
