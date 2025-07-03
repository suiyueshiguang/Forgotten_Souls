using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioManager
{
    [SerializeField] private float sfxMinimumDistance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    private Dictionary<string, AudioSource> sfxDatabase;
    private Dictionary<string, AudioSource> bgmDatabase;

    [Header("BGM内容")]
    [SerializeField] private bool playBgm;
    [SerializeField] private string currencyBgmName;

    private bool canPlaySFX;
    
    private void Awake()
    {
        if (ServiceLocator.GetService<IAudioManager>() == null)
        {
            ServiceLocator.Register<IAudioManager>(this);
        }

        sfxDatabase = new Dictionary<string, AudioSource>();
        bgmDatabase = new Dictionary<string, AudioSource>();

        AllowSFX();

        StartCoroutine(InitializedDatabase());
    }

    private void Start()
    {
        if (currencyBgmName != null)
        {
            if (!playBgm && bgmDatabase.ContainsKey(currencyBgmName))
            {
                bgmDatabase[currencyBgmName].Stop();
            }
            if (playBgm && !bgmDatabase[currencyBgmName].isPlaying)
            {
                PlayBGM(currencyBgmName);
            }
        }
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    private IEnumerator InitializedDatabase()
    {
        foreach (AudioSource audio in bgm)
        {
            bgmDatabase.Add(audio.name, audio);
        }

        yield return null;

        foreach (AudioSource audio in sfx)
        {
            sfxDatabase.Add(audio.name, audio);
        }
    }

    public void PlaySFX(string _sfxName, Transform _source)
    {
        if (!canPlaySFX)
        {
            return;
        }

        if (_source != null && Vector2.Distance(ServiceLocator.GetService<IPlayerManager>().GetPlayer().transform.position, _source.position) > sfxMinimumDistance)
        {
            return;
        }

        if (sfxDatabase.TryGetValue(_sfxName, out AudioSource sfxSource))
        {
            sfxSource.pitch = Random.Range(.9f, 1.1f);
            sfxSource.minDistance = 5f;
            sfxSource.maxDistance = sfxMinimumDistance;
            sfxSource.Play();
        }
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    /// <param name="_sfxName">音效名字</param>
    public void StopSFX(string _sfxName) => sfxDatabase[_sfxName].Stop();

    public void StopSFXWithTime(string _sfxName)
    {
        if (sfxDatabase.TryGetValue(_sfxName, out AudioSource sfxSource))
        {
            if (sfxSource != null && gameObject.activeInHierarchy)
            {
                StartCoroutine(DecreaseVolume(sfxSource));
            }
        }
    }

    /// <summary>
    /// 逐渐降低音量
    /// </summary>
    /// <param name="_audio">输入音量的下标</param>
    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * 0.3f;
            yield return new WaitForSeconds(.6f);

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;

                break;
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="_bgmName">背景音乐名称</param>
    public void PlayBGM(string _bgmName)
    {
        if (bgmDatabase.TryGetValue(_bgmName, out AudioSource bgmSource))
        {
            //如果当前播放的和预计要播放的BGM不一样，则暂停当前的BGM
            if (!currencyBgmName.Equals(_bgmName))
            {
                bgmDatabase[currencyBgmName].Stop();
            }

            //播放新的背景音乐
            currencyBgmName = _bgmName;
            bgmDatabase[currencyBgmName].Play();
        }
    }

    private void AllowSFX() => canPlaySFX = true;
}
