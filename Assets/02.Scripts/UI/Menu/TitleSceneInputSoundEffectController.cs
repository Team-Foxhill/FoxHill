using FoxHill.Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using static MenuInputAction;

public class TitleSceneInputSoundEffectController : MonoBehaviour, ITitleSceneActions, ITitleSceneSettingsActions, IVolumeAdjustable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _keySound;


    public void OnSwitchMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayKeySound(0);
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayKeySound(1);
        }
    }

    public void OnDeselect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayKeySound(2);
        }
    }

    private void PlayKeySound(int id)
    {

        if (_audioSource != null && _keySound[id] != null)
        {
            _audioSource.PlayOneShot(_keySound[id]);
        }
    }

    public void OnVolumeChanged(float volume)
    {
        _audioSource.volume = volume;
    }
}