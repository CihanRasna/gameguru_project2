using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        private static readonly int Success = Animator.StringToHash("Success");
        private static readonly int Fail = Animator.StringToHash("Fail");
        private static readonly int Play = Animator.StringToHash("Play");
        [SerializeField] private Animator cameraAnimator;
        
        public void LevelStartCamTransition()
        {
            cameraAnimator.SetTrigger(Play);
        }

        public void LevelFailCamTransition()
        {
            cameraAnimator.SetTrigger(Fail);
        }
        public void LevelCompleteCamTransition()
        {
            cameraAnimator.SetTrigger(Success);
        }
    }
}