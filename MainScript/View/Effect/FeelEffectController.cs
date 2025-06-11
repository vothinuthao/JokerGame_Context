using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Effect
{
    [Serializable]
    public sealed class OriginalObjectTransform
    {
        public GameObject objTarget;
        public Vector3 originalRectTrans;
    }
    public class FeelEffectController : MonoBehaviour
    {
        [SerializeField] private MMF_Player mmfPlayer;

        [SerializeField] private List<OriginalObjectTransform> listTrans = new List<OriginalObjectTransform>();
        
        private void Start()
        {
            mmfPlayer ??= GetComponentInChildren<MMF_Player>();
            if (listTrans == null) return;
            foreach (var obj in listTrans)
            {
                obj.objTarget.transform.localPosition = obj.originalRectTrans;
            }

        }

        public void OnPlayPerCall(float pauseDuration)
        {
            StartCoroutine(OnCall(pauseDuration));
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator OnCall(float pauseDuration)
        {
            yield return new WaitForSeconds(pauseDuration);
            mmfPlayer.PlayFeedbacks();
        }
        
    }
}