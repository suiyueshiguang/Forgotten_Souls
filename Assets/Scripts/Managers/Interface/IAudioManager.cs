using UnityEngine;

public interface IAudioManager
{
    public void PlaySFX(string _sfxName, Transform _source);
    public void StopSFX(string _sfxName);
    public void StopSFXWithTime(string _sfxName);
    public void PlayBGM(string _bgmName);
}
