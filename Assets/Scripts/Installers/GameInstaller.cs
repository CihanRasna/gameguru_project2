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
            //gameplay
            Container.Bind<MovingPlatform>().FromInstance(platformPrefab).AsSingle();
            Container.Bind<ICharacter>().FromInstance(characterInScene).AsSingle();
            Container.Bind<InputHandler>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FinishPlatform>().FromInstance(finishPlatformPrefab).AsSingle();

            //audio
            var audioGo = new GameObject("NoteAudio");
            var audioSource = audioGo.AddComponent<AudioSource>();
            Container.Bind<AudioClip>().FromInstance(noteClip).AsSingle();
            Container.Bind<AudioSource>().FromInstance(audioSource).AsSingle();

            //settings
            Container.BindInstance(gameSettings).AsSingle();

            // Managers
            Container.Bind<IGameManager>().To<GameManager>().AsSingle();
            Container.Bind<CameraManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IAudioManager>().To<AudioManager>().AsSingle();
            Container.Bind<IPlatformSpawner>().To<PlatformSpawner>().AsSingle();
        }
    }
}