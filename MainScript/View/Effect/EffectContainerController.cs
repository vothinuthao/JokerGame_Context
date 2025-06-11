using Core.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Effect
{
    public class EffectContainerController : ManualSingletonMono<EffectContainerController>
    {
        [SerializeField] private GameObject objContainer;
        
        [SerializeField]
        private UnityEvent onEventShake;

        
        public void OnShakeContainer()
        {
            onEventShake?.Invoke();
        }
    }
}
