using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace TestScript
{
    public class CardAttackEffect : MonoBehaviour
    {
        public GameObject[] enemies;
        public GameObject[] cards;
        public ParticleSystem currentEffect;
        public Transform enemyTarget;
        public float moveDuration = 1f;
        public Ease moveEase = Ease.OutQuint;
        public float explosionDistance = 0.3f;
        public void TriggerAttack()
        {
            foreach (GameObject card in cards)
            {
                card.transform.DOMove(enemyTarget.position, moveDuration).SetEase(moveEase)
                    .OnUpdate(() =>
                    {
                        if (Vector3.Distance(card.transform.position, enemyTarget.position) <= explosionDistance)
                        {
                            ExplodeCard(card);
                        }
                    })
                    .OnComplete(() =>{});
                Vector3 direction = enemyTarget.position - card.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                card.transform.DORotate(new Vector3(0, 0, angle), moveDuration).SetEase(moveEase); 
            }
            
        }
        private void ExplodeCard(GameObject card)
        {
            card.transform.DOKill();
            Destroy(card);
        }

        public void OnClickExplode()
        {
            if (currentEffect != null)
            {
                var ps = currentEffect;
                if (ps.isEmitting)
                {
                    ps.Stop(true);
                }
                else
                {
                    if (!currentEffect.gameObject.activeSelf)
                    {
                        currentEffect.gameObject.SetActive(true);
                    }
                    else
                    {
                        ps.Play(true);
                    }
                }
            }
        }
        
        
    }
}