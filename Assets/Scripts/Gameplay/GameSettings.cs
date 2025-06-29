using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Perfect Match")]
        [Range(0f, 1f)]
        public float perfectTolerance = 0.1f;
        public float basePitch = 1f;
        public float pitchStepPerPerfect = 0.1f;
        public float maxPitch = 2f;

        [Header("Platform")]
        public float initialPlatformWidth = 3f;
        public float minPlatformWidth = 0.5f;
        public float moveSpeed = 2f;
        
        [Header("Platform Colors")]
        public List<Color> platformColors;

    }
}