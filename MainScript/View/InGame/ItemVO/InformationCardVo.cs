using System;
using System.Linq;
using Core.Entity;
using DG.Tweening;
using Entity;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class InformationCardVo : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectParent;
        [SerializeField]
        private Image imgCard;
        [SerializeField]
        private GameObject objUnAvailable;
        [SerializeField]
        private GameObject objDisable;
        [SerializeField]
        private GameObject objHighLight;
        [SerializeField]
        private GameObject objDisableCard;
        
        [SerializeField] private MMF_Player mmfUpdateEffect;

        // private void Start()
        // {
        //     objHighLight.SetActive(false);
        // }

        public void SetData(PokerCard poker)
        {
            var listDeck = PlayCardController.Instance.ListCardInDeck;
            var available = listDeck.Any(x => x == poker);
            objUnAvailable.SetActive(!available);
            var sptSuit = SpritesManager.Instance.GetSpritesByRankAndSuit(poker.PokerRank,poker.PokerSuit);
            imgCard.sprite = sptSuit;
            objHighLight.SetActive(false);
            objDisableCard.SetActive(poker.isDisableCard);
        }
        public void SetData(RankCard rank, SuitCard suit, bool isSelect)
        {
            var sptSuit = SpritesManager.Instance.GetSpritesByRankAndSuit(rank,suit);
            imgCard.sprite = sptSuit;
            if (isSelect)
            {
                rectParent.DOAnchorPos(new Vector2(-30, 30), 0.1f); 
                objHighLight.SetActive(true);
            }
            else
            {
                rectParent.DOAnchorPos(new Vector2(0, 0), 0.1f); 
                objHighLight.SetActive(false);
            }
        }
    }
}
