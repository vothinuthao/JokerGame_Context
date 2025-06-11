using System.Collections.Generic;
using Core.Utils;
using DG.Tweening;
using Frameworks.Utils;
using Manager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Visual.Cards;

namespace InGame
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject prefabEmpty;

        [FoldoutGroup("Show UI")] [SerializeField]
        private TextMeshProUGUI txtAmountJokerCount;
        
        // visual card 
        [FoldoutGroup("Visual Card Drag")]
        [SerializeField] private CardVisual selectedCard;
        [FoldoutGroup("Visual Card Drag")]
        [SerializeReference] private RectTransform rect;
        [FoldoutGroup("Visual Card Drag")]
        [SerializeField] private bool tweenCardReturn = true;
        public List<CardVisual> _listCardVisuals = new List<CardVisual>();
        private bool _isCrossing = false;
        
        private void OnEnable()
        {
            JokerCardController.Instance.OnChangeJokerCard += OnChangeJokerCard;
        }

        private void OnDisable()
        {
            JokerCardController.Instance.OnChangeJokerCard -= OnChangeJokerCard;
        }

        private void Awake()
        {
            JokerCardController.Instance.OnInitJokerCard();
        }
        
        public void OnInit()
        {
            JokerCardController.Instance.UpdateListJoker();
            prefab.SetActive(false);
            GenerateOwnerCard();
        }

        private void OnChangeJokerCard()
        {
            GenerateOwnerCard();
        }
        #region  local Method
        private void GenerateOwnerCard()
        {
            var listJokerSo = JokerCardController.Instance.ListJokerCardOwner;
            GameObjectUtils.Instance.ClearAllChild(content.gameObject);
            JokerCardController.Instance.ClearAllScript();
            _listCardVisuals = new List<CardVisual>();
            for (var i = 0; i < listJokerSo.Count; i++)
            {
                var joker = listJokerSo[i];
                GameObject objEmpty = GameObjectUtils.Instance.SpawnGameObject(content, prefabEmpty);
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(objEmpty.transform, prefab);
                JokerCardVO script = obj.GetComponent<JokerCardVO>();
                script.SetData(joker);
                script.OnInit(OnUpdate);
                JokerCardController.Instance.AddScript(script);
                obj.SetActive(true);
                
                
                CardVisual scriptVisual = obj.GetComponent<CardVisual>();
                _listCardVisuals.Add(scriptVisual);
                scriptVisual.BeginDragEvent.AddListener(BeginDrag);
                scriptVisual.EndDragEvent.AddListener(EndDrag);
                scriptVisual.name = i.ToString();
            }

            OnShowUI();
        }
        private void OnUpdate(int id)
        {
            
        }

        private void OnShowUI()
        {
            int jokerOwner = PlayerDataManager.Instance.Property.TotalOwnerJoker.Count;
            int maxJokerSlot = PlayerDataManager.Instance.Property.MaxJokerSlots;
            txtAmountJokerCount.text = $"{jokerOwner}/{maxJokerSlot}";
        }
        
        private void BeginDrag(CardVisual card)
        {
            selectedCard = card;
        }
        void EndDrag(CardVisual card)
        {
            if (selectedCard == null)
                return;
            selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0,selectedCard.selectionOffset,0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);
            rect.sizeDelta += Vector2.right;
            rect.sizeDelta -= Vector2.right;
            selectedCard = null;
            
            JokerCardController.Instance.ReNewScript();
            var getAllChild = content.GetAllChilds();
            List<int> listInt = new List<int>();
            foreach (var obj in getAllChild)
            {
                var script = obj.GetChild(0).GetComponent<JokerCardVO>();
                JokerCardController.Instance.AddScript(script);
                listInt.Add(script.JokerData.ID);
            }
            PlayerDataManager.Instance.Property.ResetIndexJoker(listInt);
        }
        void Update()
        {
            if (!selectedCard)
                return;
            if (_isCrossing)
                return;
            for (int i = 0; i < _listCardVisuals.Count; i++)
            {
                if (selectedCard.transform.position.x > _listCardVisuals[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() < _listCardVisuals[i].ParentIndex())
                    {
                        SwapIndex(i);
                        break;
                    }
                }
                if (selectedCard.transform.position.x < _listCardVisuals[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() > _listCardVisuals[i].ParentIndex())
                    {
                        SwapIndex(i);
                        break;
                    }
                }
            }
        }
        private void SwapIndex(int index)
        {
            _isCrossing = true;
            Transform focusedParent = selectedCard.transform.parent;
            Transform crossedParent = _listCardVisuals[index].transform.parent;
            _listCardVisuals[index].transform.SetParent(focusedParent);
            _listCardVisuals[index].transform.localPosition = _listCardVisuals[index].selected ? new Vector3(0, _listCardVisuals[index].selectionOffset, 0) : Vector3.zero;
            selectedCard.transform.SetParent(crossedParent);
            _isCrossing = false;
            
            var listOwnerJoker = JokerCardController.Instance.ListJokerCardOwner;
            int focusedParentIndex = selectedCard.ParentIndex();
            listOwnerJoker.Swap(focusedParentIndex, index);
        }
        #endregion
    }
}
