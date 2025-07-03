using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;

    [Header("弹出文本效果")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("闪光特效")]
    [SerializeField] private Material hitMat;
    [SerializeField] private float flashDuration;
    private Material originalMat;

    [Header("debuff颜色")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color chillColor;
    [SerializeField] private Color[] shockColor;

    [Header("粒子效果")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("击打效果")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject criticalHitFx;

    private GameObject myHealthBar;

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;

        myHealthBar = GetComponentInChildren<UI_HealthBar>().gameObject;
    }

    /// <summary>
    /// 向调用者弹出信息（像：技能还在冷却）
    /// </summary>
    /// <param name="_text">警告信息</param>
    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(2, 5);

        Vector3 positionOffset = new Vector3(randomX, randomY, 0);
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            myHealthBar.SetActive(false);
            sr.color = Color.clear;
        }
        else
        {
            myHealthBar.SetActive(true);
            sr.color = Color.white;
        }
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }

    public void IgniteFXFor(float _seconds)
    {
        //可以将下面的以及其他两个统一起来，放在函数或类中，方便后续的更新
        igniteFx.Play();

        InvokeRepeating("IginiteColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFXFor(float _seconds)
    {
        chillFx.Play();

        InvokeRepeating("ChillColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFXFor(float _seconds)
    {
        shockFx.Play();

        InvokeRepeating("ShockColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IginiteColorFX()
    {
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
        }
        else
        {
            sr.color = igniteColor[1];
        }
    }

    private void ChillColorFX()
    {
        if (sr.color != chillColor)
        {
            sr.color = chillColor;
        }
    }

    private void ShockColorFX()
    {
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    public void CreateHitFx(Transform _target, bool _critical)
    {
        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(-0.5f, 0.5f);
        float zRotation = Random.Range(-90, 90);

        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);

        GameObject hitPrefer = hitFx;

        if (_critical)
        {
            hitPrefer = criticalHitFx;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
            {
                yRotation = 150;
            }

            hitFxRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFX = Instantiate(hitPrefer, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity, _target);

        newHitFX.transform.Rotate(hitFxRotation);

        Destroy(newHitFX, .5f);
    }
}
