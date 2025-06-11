using System;
using System.Collections;
using Core.Manager;
using Core.Utils;
using Frameworks.UIPopup;
using InGame;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class PopupMapProgression : UIPopup
    {
        [FoldoutGroup("Spawn Progression")] 
        [SerializeField] private ScrollRect scrollRect;
        [FoldoutGroup("Spawn Progression")] 
        [SerializeField] private Transform transformParentSpawn;
        [FoldoutGroup("Spawn Progression")] 
        [SerializeField] private GameObject gameObjectItemSpawn;
        [SerializeField] private float movingSpeed;
        private RoundInfoVo _objCurrent;
        private RoundInfoVo _objNextCurrent;
        private bool _isMove = false;
        private float _scrollDuration = 0.5f;
        private void Start()
        {
            gameObjectItemSpawn.SetActive(false);
            
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnShown()
        {
            base.OnShown();
            _isMove = (bool)Parameter;
            OnCreateProgression();
            TouchManager.Instance.SetCanTouch(false);
            OnAnimation();
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnHidden()
        {
            base.OnHidden();
            TouchManager.Instance.SetCanTouch(true);
        }
        public void OnClickHide()
        {
            Hide();
        }
        private void OnCreateProgression()
        {
            GameObjectUtils.Instance.ClearAllChild(transformParentSpawn.gameObject);
            var configRound = ConfigManager.configPlayRound.Records;
            int exactlyRound = PlayerDataManager.Instance.Property.Round % (ConfigManager.configPlayRound.Records.Count - 1);
            for (int i = 1; i < configRound.Count; i++)
            {
                var item = configRound[i];
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transformParentSpawn, gameObjectItemSpawn);
                RoundInfoVo script = obj.GetComponent<RoundInfoVo>();
                script.SetDataRoundProgress(item);
                obj.SetActive(true);
                obj.name = "Round: " + i;
                if (item.round == (exactlyRound - 1 <= 0 ? 1 : exactlyRound - 1))
                    _objCurrent = script;
                if (item.round == exactlyRound)
                    _objNextCurrent = script;

            }
          
        }
        IEnumerator WaitAndScroll()
        {
            yield return new WaitForSeconds(0.1f);
            ScrollToSelectedElement();
        }
        private void ScrollToSelectedElement()
        {
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            float round2YPos = _objCurrent.rectTransformParent.anchoredPosition.y;
            int exactlyRound = PlayerDataManager.Instance.Property.Round %
                              (ConfigManager.configPlayRound.Records.Count - 1);
            float targetYPos = -round2YPos + viewportHeight / exactlyRound - _objCurrent.rectTransformParent.rect.height / 2f;
            Canvas.ForceUpdateCanvases(); 
            scrollRect.verticalNormalizedPosition = 1f - (targetYPos / contentHeight);
        }

        private void OnAnimation()
        {
            
            if (_isMove)
            {
                StartCoroutine(StartScrolling());
            }
            else
            {
                StartCoroutine(WaitAndScroll());
            }
        }
        private IEnumerator StartScrolling()
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ScrollToRound());
            // StartCoroutine(WaitAndScroll());
        }

        private IEnumerator ScrollToRound()
        {
            yield return new WaitForSeconds(0.1f);
            ScrollToSelectedElement();
            yield return new WaitForSeconds(0.5f);
            _objCurrent.ChangeCurrent(false);
            float elapsedTime = 0f;
            Vector2 startingPos = scrollRect.normalizedPosition;
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            float targetYPos = -_objNextCurrent.rectTransformParent.anchoredPosition.y - viewportHeight / 10f - _objNextCurrent.rectTransformParent.rect.height / 10f;
            Vector2 targetPos = new Vector2(startingPos.x, 1f - (targetYPos / contentHeight));

            while (elapsedTime < _scrollDuration)
            {
                scrollRect.normalizedPosition = Vector2.Lerp(startingPos, targetPos / 3f, elapsedTime / _scrollDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
           
            yield return new WaitForSeconds(0.4f);
            _objNextCurrent.ChangeCurrent(true);
            scrollRect.normalizedPosition = targetPos / 3f;
        }
    }
}
