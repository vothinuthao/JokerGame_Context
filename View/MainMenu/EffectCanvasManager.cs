
using System;
using System.Collections;
using System.Collections.Generic;
using Core.Entity;
using Core.Utils;
using InGame;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class EffectCanvasManager : ManualSingletonMono<EffectCanvasManager>
    {
        [FoldoutGroup("Material")]
        [SerializeField] private Material materialRed;
        [FoldoutGroup("Material")]
        [SerializeField] private Material materialGreen;
        [FoldoutGroup("Material")]
        [SerializeField] private Material materialBlue;
        [FoldoutGroup("Material")]
        [SerializeField] private Material materialMain;
        [FoldoutGroup("Background")]
        [SerializeField] private Image imgBackground;
        
        
        [FoldoutGroup("Effect Dame")]
        [SerializeField] private RectTransform objPlayerTotalDame;
        [FoldoutGroup("Effect Dame")]
        [SerializeField] private RectTransform objPlayerTotalBigDame;
        [FoldoutGroup("Effect Dame")]
        [SerializeField] private GameObject contentParent;


        private void Start()
        {
            BackgroundController.Instance.OnBackgroundChanged += OnBackgroundChanged;
            objPlayerTotalDame.gameObject.SetActive(false);
            objPlayerTotalBigDame.gameObject.SetActive(false);
        }
        private void OnApplicationQuit()
        {
            BackgroundController.Instance.OnBackgroundChanged -= OnBackgroundChanged;
        }
        private void OnBackgroundChanged(BackgroundEnum @enum)
        {
            Material mat = materialMain;
            switch (@enum)
            {
                case BackgroundEnum.None:
                    break;
                case BackgroundEnum.BackgroundBlue:
                    mat = materialBlue;
                    break;
                case BackgroundEnum.BackgroundGreen:
                    mat = materialGreen;
                    break;
                case BackgroundEnum.BackgroundRed:
                    mat = materialRed;
                    break;
                case BackgroundEnum.BackgroundMain:
                    mat = materialMain;
                    break;
                
            }
            imgBackground.material = mat;
        }

        public void ShowEffectTotalDame(RectTransform rect, float dame, bool isBigDame = false)
        {
            GameObject obj = GameObjectUtils.Instance.SpawnGameObject(contentParent.transform, !isBigDame ? objPlayerTotalDame.gameObject : objPlayerTotalBigDame.gameObject);
            var script = obj.GetComponent<TotalDameVo>();
            script.SetDataEffect(rect,dame,isBigDame);
            obj.SetActive(true);
        }
    }
}
