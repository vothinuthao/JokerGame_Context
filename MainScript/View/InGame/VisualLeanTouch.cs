using System;
using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class VisualLeanTouch : MonoBehaviour
    {
        [SerializeField] private GameObject prefabZone;
        [SerializeField] private RectTransform content;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;

        private float _widthOrigin;
        private float _heightOrigin;
        public int CurrentNumber { get; set; }

        private Action<int> _callback;

        private void Start()
        {
            _widthOrigin = content.rect.width;
            _heightOrigin = content.rect.height;
        }

        public void OnInit(int count,Action<int> callback)
        {
            _callback = callback;
            gridLayoutGroup.cellSize = new Vector2(_widthOrigin / count, _heightOrigin);
            GameObjectUtils.Instance.ClearAllChild(content.gameObject);
        }
        public void SpawnVisualZone(GameObject enemy, int index)
        {
            prefabZone.SetActive(false);
            // GameObjectUtils.Instance.ClearAllChild(content.gameObject);
            var ob = GameObjectUtils.Instance.SpawnGameObject(content, prefabZone);
            VisualEnemyVo vo = ob.GetComponent<VisualEnemyVo>();
            vo.SetData(index,enemy, CallBackID);
            ob.SetActive(true);
        }

        private void CallBackID(int id )
        {
            CurrentNumber = id;
            _callback?.Invoke(id);
        }
    }
}
