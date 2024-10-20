using FoxHill.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneAudioPlayer : MonoBehaviour, IVolumeAdjustable
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _audioClips;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAudioSequence());
        SoundVolumeManager.Register(this);
    }

    private void OnDestroy()
    {
        SoundVolumeManager.Unregister(this);
    }

    private IEnumerator PlayAudioSequence()
    {
        yield return PlayClipAndWait(_audioClips[0], true);  // PlayOneShot 사용
        while (true)
        {
            yield return PlayClipAndWait(_audioClips[1], false); // 일반 재생 사용
        }
    }

    private IEnumerator PlayClipAndWait(AudioClip clip, bool usePlayOneShot)
    {
        if (usePlayOneShot)
            _audioSource.PlayOneShot(clip);
        else
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        yield return new WaitWhile(() => _audioSource.isPlaying);
    }

    public void OnVolumeChanged(float volume)
    {
        _audioSource.volume = volume;
    }
}
