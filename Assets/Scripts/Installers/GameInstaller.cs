using Gameplay;
using Managers;
using UnityEngine;
using Zenject;
using CharacterController = Gameplay.CharacterController;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public GameSettings gameSettings;
        public MovingPlatform platformPrefab;
        public CharacterController characterInScene;
        public AudioClip noteClip;
        public FinishPlatform finishPlatformPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ICharacter>().FromInstance(characterInScene).AsSingle();
            Container.Bind<InputHandler>().FromComponentInHierarchy().AsSingle();

            //audio
            var audioGo = new GameObject("NoteAudio");
            var audioSource = audioGo.AddComponent<AudioSource>();
            Container.Bind<AudioClip>().FromInstance(noteClip).AsSingle();
            Container.Bind<AudioSource>().FromInstance(audioSource).AsSingle();
            Container.Bind<IAudioManager>().To<AudioManager>().AsSingle();
            
            // Spawner bind
            Container.Bind<IPlatformSpawner>().To<PlatformSpawner>().AsSingle().WithArguments(platformPrefab);

            // GameManager
            Container.Bind<FinishPlatform>().FromInstance(finishPlatformPrefab).AsSingle();
            Container.BindInstance(gameSettings).AsSingle();
            Container.Bind<IGameManager>().To<GameManager>().AsSingle();
        }
    }
}