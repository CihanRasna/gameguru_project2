using Gameplay;
using UnityEngine;

namespace Managers
{
    public class AudioManager : IAudioManager
    {
        private readonly AudioSource audioSource;
        private readonly AudioClip noteClip;
        private readonly GameSettings settings;

        public AudioManager(AudioSource source, AudioClip clip, GameSettings gameSettings)
        {
            audioSource = source;
            noteClip = clip;
            settings = gameSettings;
        }

        public void PlayNote(int perfectCombo)
        {
            var pitch = Mathf.Clamp(
                settings.basePitch + perfectCombo * settings.pitchStepPerPerfect,
                settings.basePitch,
                settings.maxPitch
            );

            audioSource.pitch = pitch;
            audioSource.PlayOneShot(noteClip);
        }

        public void PlayFailure()
        {
            audioSource.pitch = settings.basePitch;
            audioSource.PlayOneShot(noteClip);
        }
    }
}