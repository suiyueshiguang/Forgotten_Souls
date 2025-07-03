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

    [Header("BGM����")]
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
    /// ��ʼ������
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
    /// ��ͣ��Ч
    /// </summary>
    /// <param name="_sfxName">��Ч����</param>
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
    /// �𽥽�������
    /// </summary>
    /// <param name="_audio">�����������±�</param>
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
    /// ���ű�������
    /// </summary>
    /// <param name="_bgmName">������������</param>
    public void PlayBGM(string _bgmName)
    {
        if (bgmDatabase.TryGetValue(_bgmName, out AudioSource bgmSource))
        {
            //�����ǰ���ŵĺ�Ԥ��Ҫ���ŵ�BGM��һ��������ͣ��ǰ��BGM
            if (!currencyBgmName.Equals(_bgmName))
            {
                bgmDatabase[currencyBgmName].Stop();
            }

            //�����µı�������
            currencyBgmName = _bgmName;
            bgmDatabase[currencyBgmName].Play();
        }
    }

    private void AllowSFX() => canPlaySFX = true;
}
