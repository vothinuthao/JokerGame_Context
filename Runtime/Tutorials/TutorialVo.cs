using System;
using Core.Controller.Observer;
using Core.Utils;
using Frameworks.Scripts;
using Manager;
using MoreMountains.Feedbacks;
using TMPro;
using UI.BaseColor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tutorials
{
    public class TutorialVo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtDescription;
        private TutorialsName _data;
        [SerializeField] private Transform transSpawn;
        
        private bool _isComplete = false;
        public void OnInitData(TutorialsName data)
        {
            _data = data;
            //test
            OnCompleteTutorial();
        }
        public void ShowPanel()
        {
            this.gameObject.SetActive(true);
            if(txtDescription != null)
                txtDescription.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(_data.content);
            if (_data.isInhere)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transSpawn, _data.objSpawnTut);
                obj.transform.position = _data.targetTransform.position;
                obj.SetActive(true);
            }

            if (_data.isShowObjectByCanvas && PlayerDataManager.Instance.Property.CurrentIdTutorial == (int)TutorialState.Tutorial09)
            {
                _data.canvasTarget.overrideSorting = true;
                _data.canvasTarget.sortingOrder = 5;
            }
            else
            {
                TutorialsView.Instance.SetCanvasObjectTut(TutorialState.Tutorial09,  false);
            }
        }

        private void OnCompleteTutorial()
        {
            _isComplete = true;
        }
        

        public void HidePanel()
        {
            this.gameObject.SetActive(false);
        }

        public void OnClickNextTutorial()
        {
            this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)_data.idTutorial != 0 ? (int)_data.idTutorial  + 1 : 2 ,IsCompleted = _isComplete});
            AudioManager.Instance.PlaySFX(AudioName.SoundClick);
            
        }
    }
}