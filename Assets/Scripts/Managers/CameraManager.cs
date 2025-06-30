using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Animator cameraAnimator;

        public void LevelCompleteCamTransition()
        {
            cameraAnimator.SetTrigger("Finish");
        }
    }
}