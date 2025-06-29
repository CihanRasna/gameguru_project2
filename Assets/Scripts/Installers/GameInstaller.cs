using Gameplay;
using Managers;
using UnityEngine;
using Zenject;
using CharacterController = Gameplay.CharacterController;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public MovingPlatform platformPrefab;
        public CharacterController characterInScene;
        public AudioClip noteClip;

        public override void InstallBindings()
        {
            Container.Bind<ICharacter>().FromInstance(characterInScene).AsSingle();
            Container.Bind<AudioClip>().FromInstance(noteClip).AsSingle();

            var audioGo = new GameObject("NoteAudio");
            var audioSource = audioGo.AddComponent<AudioSource>();
            Container.Bind<AudioSource>().FromInstance(audioSource).AsSingle();

            // Spawner bind
            Container.Bind<IPlatformSpawner>().To<PlatformSpawner>().AsSingle().WithArguments(platformPrefab);

            // GameManager
            Container.Bind<IGameManager>().To<GameManager>().AsSingle();
        }
    }
}