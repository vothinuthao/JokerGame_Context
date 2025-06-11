using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tutorials
{
    [System.Serializable]
    public class TutorialsName
    {
        public TutorialState idTutorial;
        public TutorialVo tutorialVo;
        [MultiLineProperty(3)]
        public string content;
        public bool isInhere;
        public bool isShowObjectByCanvas;
        [SerializeField]
        private bool isComplete = false;
            
        [ShowIfGroup("isInhere")]
        [FoldoutGroup("isInhere/Target Settings")]
        public GameObject objSpawnTut;
        [ShowIfGroup("isInhere")]
        [FoldoutGroup("isInhere/Target Settings")]
        [ShowIfGroup("isInhere")]
        [FoldoutGroup("isInhere/Target Settings")]
        public Transform targetTransform;
        
        
        [ShowIfGroup("isShowObjectByCanvas")]
        public Canvas canvasTarget;

        public bool IsComplete => isComplete;

        public void SetCompleteTut()
        {
            if(isComplete)
                return;
            isComplete = true;
        }
    }
    public enum TutorialState
    {
        None = 0,             // 0
        Tutorial01 = 1,       // 1
        Tutorial02 = 2,       // 2
        Tutorial03 = 3,       // 3
        Tutorial04 = 4,       // 4
        Tutorial05 = 5,       // 5
        Tutorial06 = 6,       // 6
        Tutorial07 = 7,       // 7
        Tutorial08 = 8,       // 8
        Tutorial09 = 9,       // 9
        Tutorial10 = 10,      // 10
        Tutorial10_B = 11,    // 11
        Tutorial11 = 12,      // 12
        Tutorial12 = 13,      // 13
        Tutorial12_B = 14,    // 14
        Tutorial13 = 15,      // 15
        Completed = 16        // 16
    }

}
