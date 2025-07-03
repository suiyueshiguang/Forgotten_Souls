using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    private new Light2D light;
    private float timeInGame;//游戏时间分成了六段，即清晨，早上，中午，傍晚，晚上，午夜

    [Header("现实时间中的每秒对应游戏多少分钟")]
    [SerializeField] private float realSecondsPerGameMinute = 1;

    [Header("光照颜色")]
    [SerializeField] private Gradient lightColor;

    private AnimationCurve intensityCurve;

    private void Start()
    {
        light = GetComponent<Light2D>();

        InitializeCurve();
    }

    private void Update()
    {
        //更新游戏时间
        timeInGame += Time.deltaTime * realSecondsPerGameMinute / 1440f;

        timeInGame %= 1;

        light.color = lightColor.Evaluate(timeInGame);
        light.intensity = intensityCurve.Evaluate(timeInGame);
    }

    private void InitializeCurve()
    {
        intensityCurve = new AnimationCurve();

        //清晨起点
        intensityCurve.AddKey(new Keyframe(0.0f, 0.5f));
        //清晨终点
        intensityCurve.AddKey(new Keyframe(0.16f, 1f));
        //上午
        intensityCurve.AddKey(new Keyframe(0.33f, 1.5f));
        //中午
        intensityCurve.AddKey(new Keyframe(0.5f, 1.5f));
        //傍晚
        intensityCurve.AddKey(new Keyframe(0.66f, 0.8f));
        //晚上
        intensityCurve.AddKey(new Keyframe(0.83f, 0.3f));
        //午夜
        intensityCurve.AddKey(new Keyframe(1.0f, 0.5f));

        intensityCurve.postWrapMode = WrapMode.Loop;
    }
}
