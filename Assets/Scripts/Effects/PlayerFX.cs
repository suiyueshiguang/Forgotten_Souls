using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : EntityFX
{
    private Player player;

    [Header("后续图像效果")]
    [SerializeField] private GameObject afterImagePrefer;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;

    [Header("屏幕抖动效果")]
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;
    private CinemachineImpulseSource screenShake;

    [Space]
    [SerializeField] private ParticleSystem dustFx;

    protected override void Start()
    {
        base.Start();

        player = ServiceLocator.GetService<IPlayerManager>().GetPlayer();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 屏幕抖动
    /// </summary>
    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    /// <summary>
    /// 玩家闪避的多重残影
    /// </summary>
    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;

            GameObject newAfterImage = Instantiate(afterImagePrefer, transform.position, transform.rotation);

            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }
    }

    public void PlayerDustFX()
    {
        if (dustFx != null)
        {
            dustFx.Play();
        }
    }
}
