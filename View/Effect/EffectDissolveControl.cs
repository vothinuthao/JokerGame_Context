using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Effect
{
    public class EffectDissolveControl : MonoBehaviour
    {
        [SerializeField] private Image imageMaterial;
        [SerializeField] private Material targetMaterial;
        [SerializeField] private float fadeDuration = 0.5f;
        private Material _currentMaterial;
        
        [SerializeField] private UnityEvent onActionFinishEffect;
        
        private void Start()
        {
            imageMaterial ??= this.GetComponent<Image>();
            Material newMaterial = new Material(targetMaterial);
            imageMaterial.material = newMaterial;
            _currentMaterial = newMaterial;
            _currentMaterial.SetFloat("_FadeAmount", 1);
        }
        
        public void OnPlayDissolve()
        {
            float elapsedTime = 0f;
            float startValue = 1f;
            float endValue = 0f;

            DOTween.To(() => _currentMaterial.GetFloat("_FadeAmount"),
                x => _currentMaterial.SetFloat("_FadeAmount", x), 
                0f, fadeDuration)
                .OnComplete(() =>
                {
                    onActionFinishEffect?.Invoke();
                });
            
            
        }
    }
}
