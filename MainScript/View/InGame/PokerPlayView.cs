using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entity;
using Core.Utils;
using DG.Tweening;
using Frameworks.Utils;
using MoreMountains.Tools;
using Sirenix.Utilities;
using UnityEngine;

namespace InGame
{
    public class PokerPlayView : MonoBehaviour
    {
        [SerializeField] private GameObject objPrefab;
        [SerializeField] private Transform transParent;

        private Dictionary<int, Transform> _dicSaveLocalObject = new Dictionary<int, Transform>();
        private void Start()
        {
            objPrefab.SetActive(false);
        }
        public void SpawnBasePokerHand()
        {
            var handSlot = PlayCardController.Instance.ListCardOnSelect;
            GameObjectUtils.Instance.ClearAllChild(transParent.gameObject);
            _dicSaveLocalObject.Clear();
            for (int i = 0; i < handSlot.Count(); i++)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transParent, objPrefab);
                obj.name = $"{i}";
                if (!_dicSaveLocalObject.ContainsKey(i))
                    _dicSaveLocalObject.Add(i,obj.transform);
                obj.SetActive(true);
            }
        }
        
        public void AttachChildToParentByIndex(int index, GameObject obj)
        {
            var newTrans = _dicSaveLocalObject[index];
            GameObjectUtils.Instance.MoveToNewLocation(newTrans.transform, obj.transform.AsRectTransform());
        }
        
    }
}
