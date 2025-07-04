using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Perfect Match")] [Range(0f, 1f)]
        public float perfectTolerance = 0.1f;

        public float basePitch = 1f;
        public float pitchStepPerPerfect = 0.1f;
        public float maxPitch = 2f;
        public AudioClip noteClip;
        public AudioClip badNoteClip;

        [Header("Platform")] public float initialPlatformWidth = 3f;
        public float minPlatformWidth = 0.5f;
        public float moveSpeed = 2f;

        [Header("Platform Colors")] public List<Color> platformColors;

        [Header("Character")] public float characterMoveSpeed = 2f;
        public float characterMaxSpeed = 6f;
        public float characterDistanceThreshold = 3f;
        public float characterRotationSpeed;

        [Header("Level Data")] public List<LevelDistance> levelDistances;

        public int GetStepCountForLevel(int level)
        {
            if (levelDistances == null || levelDistances.Count == 0)
                return 0;

            var index = level % levelDistances.Count;
            return levelDistances[index].stepCount;
        }

    }

    [System.Serializable]
    public class LevelDistance
    {
        public int level;
        public int stepCount;
    }
}