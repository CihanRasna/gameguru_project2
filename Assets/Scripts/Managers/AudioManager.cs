using Gameplay;
using UnityEngine;

namespace Managers
{
    public class AudioManager : IAudioManager
    {
        private readonly AudioSource _audioSource;
        private readonly AudioClip _noteClip;
        private readonly AudioClip _badNoteClip;
        private readonly GameSettings _settings;

        public AudioManager(AudioSource source, GameSettings gameSettings)
        {
            _audioSource = source;
            _noteClip = gameSettings.noteClip;
            _badNoteClip = gameSettings.badNoteClip;
            _settings = gameSettings;
        }

        public void PlayNote(int perfectCombo)
        {
            var pitch = Mathf.Clamp(
                _settings.basePitch + perfectCombo * _settings.pitchStepPerPerfect,
                _settings.basePitch,
                _settings.maxPitch
            );

            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(_noteClip);
        }

        public void PlayFailure()
        {
            _audioSource.pitch = _settings.basePitch;
            _audioSource.PlayOneShot(_badNoteClip);
        }
    }
}