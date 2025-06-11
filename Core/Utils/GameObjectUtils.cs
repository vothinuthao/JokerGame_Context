using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using InGame;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Utils
{
    public class GameObjectUtils : ManualSingletonMono<GameObjectUtils>
    {
        public GameObject SpawnGameObject(Transform parent, GameObject prefab)
        {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }
        
        public void ClearAllChild(GameObject parent)
        {
            var children = new List<GameObject>();
            foreach (Transform child in parent.transform) children.Add(child.gameObject);
            parent.transform.DetachChildren();
            children.ForEach(Destroy);
        }
        public void MoveToNewLocation(Transform newTran, RectTransform item, bool isSmooth = true)
        {
            const float duration = 0.1f;
            item.SetParent(newTran);
            item.DOMove(newTran.position, duration) 
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => 
                {
                    item.localPosition = Vector3.zero;
                    item.localRotation = Quaternion.identity;
                    item.localScale = Vector3.one;
                });

            item.DORotateQuaternion(newTran.rotation, duration).SetEase(Ease.InOutQuad);
            item.DOScale(Vector3.one, duration).SetEase(Ease.InOutQuad);
            
            Vector3 direction = newTran.position - item.transform.position; // Vector hướng
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Góc quay
            item.rotation = Quaternion.Euler(0, 0, angle); 
        
        }
        public List<int> ConvertStringToList(string str)
        {
            string[] stringArray = str.Split(';');
            List<int> numberList = stringArray.Select(int.Parse).ToList();
            return numberList;
        }
    }
}
