using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Controller.Observer;
using Core.Utils;
using DG.Tweening;
using Frameworks.Base;
using Frameworks.UIPopup;
using Frameworks.Utils;
using Manager;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tutorials;
using UnityEngine;
using Visual.Cards;

namespace InGame
{
	public class PokerHandView : MonoBehaviour
	{
		[FoldoutGroup("Visual Card")]
		[SerializeField] private GameObject objPrefab;
		[FoldoutGroup("Visual Card")]
		[SerializeField] private Transform transParent;
		[FoldoutGroup("Visual Card")]
		[SerializeField] private Transform transParentFake;
		
		// visual card 
		[FoldoutGroup("Visual Card Drag")]
		[SerializeField] private CardVisual selectedCard;
		[FoldoutGroup("Visual Card Drag")]
		[SerializeReference] private RectTransform rect;
		[FoldoutGroup("Visual Card Drag")]
		[SerializeField] private bool tweenCardReturn = true;
		public List<CardVisual> _listCardVisuals = new List<CardVisual>();
		private bool _isCrossing = false;
		// end tab
		
		private Dictionary<int, Transform> _dicSaveLocalObject = new Dictionary<int, Transform>();
		private void Start()
		{
			SpawnBasePokerHand();
			
			StartCoroutine(Frame());
			IEnumerator Frame()
			{
				yield return new WaitForSecondsRealtime(.1f);
			}
		}
		public void SpawnBasePokerHand()
		{
			var handSlot = PlayerDataManager.Instance.Property.MaxDrawCardInHand;
			_dicSaveLocalObject.Clear();
			GameObjectUtils.Instance.ClearAllChild(transParent.gameObject);
			for (int i = 0; i < handSlot; i++)
			{
				GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transParent, objPrefab);
				obj.name = $"{i}";
				if (!_dicSaveLocalObject.ContainsKey(i))
					_dicSaveLocalObject.Add(i, obj.transform);
				obj.SetActive(true);
			}
			
		}
		public void SpawnCardVisualDrag(Transform trans, int index)
		{
			var scriptVisual = trans.GetComponentInChildren<CardVisual>();
			_listCardVisuals.Add(scriptVisual);
			scriptVisual.BeginDragEvent.AddListener(BeginDrag);
			scriptVisual.EndDragEvent.AddListener(EndDrag);
			scriptVisual.name = index.ToString();
		}

		public void ClearListCardVisual()
		{
			_listCardVisuals = new List<CardVisual>();
		}
		public void AttachChildToParentByIndex(int index, GameObject obj)
		{
			// var duration = 0.1f;
			var newTrans = _dicSaveLocalObject[index];
			GameObjectUtils.Instance.MoveToNewLocation(newTrans.transform, obj.transform.AsRectTransform());
		}
		public void ClearAllChildCard()
		{
			if (_dicSaveLocalObject.Count != 0)
				_dicSaveLocalObject.ForEach(x => x.Value.transform.MMDestroyAllChildren());
		}
		public void SortPokerOnHand()
		{
			if (PlayCardController.IsInstanceValid())
			{
				var listScript = PlayCardController.Instance.ListCardOnHandScripts;
				List<Transform> listChild = new List<Transform>();
				for (var i = 0; i < _dicSaveLocalObject.Count; i++)
				{
					if (_dicSaveLocalObject[i].childCount > 0)
						listChild.Add(_dicSaveLocalObject[i].GetChild(0));
				}
				for (int i = 0; i < listScript.Count(); i++)
				{
					var item = listScript[i];
					listChild.ForEach(child =>
					{
						if (item.Equals(child.GetComponent<PokerCardVo>()))
							AttachChildToParentByIndex(i, child.gameObject);
					});
				}
			}
		}
		public Task SwapPositionCard()
		{
			var listCardOnHandScriptVo = PlayCardController.Instance.ListCardOnHandScripts;
			var getAllChildNoEmpty = new List<Transform>();
			foreach (var item in _dicSaveLocalObject)
			{
				if (item.Value.childCount > 0)
				{
					getAllChildNoEmpty.Add(item.Value.GetChild(0));
				}
			}
			if (getAllChildNoEmpty.Count == 0) return Task.CompletedTask;
			for (int i = 0; i < listCardOnHandScriptVo.Count; i++)
			{
				var sc = listCardOnHandScriptVo[i];
				foreach (var item in getAllChildNoEmpty)
				{
					if (item.GetComponent<PokerCardVo>().Equals(sc))
					{
						AttachChildToParentByIndex(i, item.gameObject);
					}
				}
			}
			return Task.CompletedTask;
		}
		public void OnShowDeck()
		{
			if (!PlayerDataManager.Instance.Property.IsCompletedTutorial && PlayerDataManager.Instance.Property.CurrentIdTutorial <=5)
				this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial06, IsCompleted = true });
			UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupInformationDeck);
		}
		
		#region  Card Visual Drag
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
			
			PlayCardController.Instance.ReNewListCardOnHand();
			var getAllChild = transParent.GetAllChilds();
			foreach (var obj in getAllChild)
			{
				var script = obj.GetChild(0).GetComponent<PokerCardVo>();
				PlayCardController.Instance.AddScriptCardOnHand(script);
			}
		}
		void Update()
		{
			if (!selectedCard)
				return;
			if (_isCrossing)
				return;
		
			for (int i = 0; i < _listCardVisuals.Count; i++)
			{
				if (_listCardVisuals[i])
				{
					if (selectedCard.transform.position.x > _listCardVisuals[i].transform.position.x)
                    {
                    	if (selectedCard.ParentIndex() < _listCardVisuals[i].ParentIndex())
                    	{
                    		Swap(i);
                    		break;
                    	}
                    }
                    if (selectedCard.transform.position.x < _listCardVisuals[i].transform.position.x)
                    {
                    	if (selectedCard.ParentIndex() > _listCardVisuals[i].ParentIndex())
                    	{
                    		Swap(i);
                    		break;
                    	}
                    }
				}
			}
		}
		void Swap(int index)
		{
			_isCrossing = true;
			Transform focusedParent = selectedCard.transform.parent;
			Transform crossedParent = _listCardVisuals[index].transform.parent;
			_listCardVisuals[index].transform.SetParent(focusedParent);
			_listCardVisuals[index].transform.localPosition = _listCardVisuals[index].selected ? new Vector3(0, _listCardVisuals[index].selectionOffset, 0) : Vector3.zero;
			selectedCard.transform.SetParent(crossedParent);
			_isCrossing = false;
		}

		#endregion
		
	}
}
