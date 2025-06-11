using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entity;
using Core.Utils;
using Entity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class ViewDeckView : MonoBehaviour
    {
        [FormerlySerializedAs("textSuitSpace")]
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitSpade;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitHeart;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitClub;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitDiamond;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitFaceCard;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitAce;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitNumberRank;
        
        [FormerlySerializedAs("textSuitSpace")]
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitSpade2;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitHeart2;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitClub2;
        [FoldoutGroup("Statistical")]
        [SerializeField] private TextMeshProUGUI textSuitDiamond2;
         
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private GameObject objPrefabCard;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private Transform transSpadeSpawn;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private Transform transHeartSpawn;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private Transform transClubSpawn;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private Transform transDiamondSpawn;
        
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private GridLayoutGroup gridDiamondSpawn;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private GridLayoutGroup gridSpadeSpawn;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private GridLayoutGroup gridHeartSpawn;
        [FoldoutGroup("Statistical Card")]
        [SerializeField] private GridLayoutGroup gridClubSpawn;
        
        [FoldoutGroup("Statistical Card Rank")]
        [SerializeField] private GameObject objPrefabRankCard;
        [FoldoutGroup("Statistical Card Rank")]
        [SerializeField] private Transform transRankCardSpawn;

        
        [FoldoutGroup("Statistical Suit Count")]
        [SerializeField] private GameObject objCountSuitByRank;
        [FoldoutGroup("Statistical Suit Count")]
        [SerializeField] private Transform transformSpadeCount;
        [FoldoutGroup("Statistical Suit Count")]
        [SerializeField] private Transform transformHeartCount;
        [FoldoutGroup("Statistical Suit Count")]
        [SerializeField] private Transform transformClubCount;
        [FoldoutGroup("Statistical Suit Count")]
        [SerializeField] private Transform transformDiamondCount;


        private void Start()
        {
            objPrefabCard.SetActive(false);
            objPrefabRankCard.SetActive(false);
            objCountSuitByRank.SetActive(false);
        }

        public void OnShow()
        {
            OnShowStatistical();
            OnShowStatisticalCard();
            OnShowStatisticalRankCard();
            OnShowStatisticalSuitCard();
        }

        private void OnShowStatistical()
        {
            var listDeck = PlayCardController.Instance.ListCardInDeck;
            textSuitSpade.text = listDeck.Count(x => x.PokerSuit == SuitCard.Spade).ToString();
            textSuitSpade2.text = listDeck.Count(x => x.PokerSuit == SuitCard.Spade).ToString();
            textSuitHeart.text = listDeck.Count(x => x.PokerSuit == SuitCard.Heart).ToString();
            textSuitHeart2.text = listDeck.Count(x => x.PokerSuit == SuitCard.Heart).ToString();
            textSuitClub.text = listDeck.Count(x => x.PokerSuit == SuitCard.Club).ToString();
            textSuitClub2.text = listDeck.Count(x => x.PokerSuit == SuitCard.Club).ToString();
            textSuitDiamond.text = listDeck.Count(x => x.PokerSuit == SuitCard.Diamond).ToString();
            textSuitDiamond2.text = listDeck.Count(x => x.PokerSuit == SuitCard.Diamond).ToString();
            textSuitFaceCard.text = listDeck.Count(x => x.PokerRank is RankCard.Jack or RankCard.Queen or RankCard.King).ToString();
            textSuitAce.text = listDeck.Count(x => x.PokerRank is RankCard.Ace).ToString();
            textSuitNumberRank.text = listDeck.Count(x => x.PokerRank is not (RankCard.Ace or RankCard.Jack or RankCard.Queen or RankCard.King)).ToString();
        }
        private void OnShowStatisticalCard()
        {
            var listFullDeck = PlayCardController.Instance.ListCardFullDeck;
            List<PokerCard> listSpade = listFullDeck.Where(x => x.PokerSuit == SuitCard.Spade).OrderBy(card => card.PokerRank).ToList();
            List<PokerCard> listHeart = listFullDeck.Where(x => x.PokerSuit == SuitCard.Heart).OrderBy(card => card.PokerRank).ToList();
            List<PokerCard> listClub = listFullDeck.Where(x => x.PokerSuit == SuitCard.Club).OrderBy(card => card.PokerRank).ToList();
            List<PokerCard> listDiamond = listFullDeck.Where(x => x.PokerSuit == SuitCard.Diamond).OrderBy(card => card.PokerRank).ToList();
            
            GameObjectUtils.Instance.ClearAllChild(transSpadeSpawn.gameObject);
            GameObjectUtils.Instance.ClearAllChild(transHeartSpawn.gameObject);
            GameObjectUtils.Instance.ClearAllChild(transClubSpawn.gameObject);
            GameObjectUtils.Instance.ClearAllChild(transDiamondSpawn.gameObject);
            
            foreach (var spade in listSpade)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transSpadeSpawn, objPrefabCard);
                var script = obj.GetComponent<InformationCardVo>();
                script.SetData(spade);
                obj.SetActive(true);
            }
            foreach (var heart in listHeart)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transHeartSpawn, objPrefabCard);
                var script = obj.GetComponent<InformationCardVo>();
                script.SetData(heart);
                obj.SetActive(true);
            }
            foreach (var club in listClub)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transClubSpawn, objPrefabCard);
                var script = obj.GetComponent<InformationCardVo>();
                script.SetData(club);
                obj.SetActive(true);
            }
            foreach (var diamond in listDiamond)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transDiamondSpawn, objPrefabCard);
                var script = obj.GetComponent<InformationCardVo>();
                script.SetData(diamond);
                obj.SetActive(true);
            }

            PerfectSize(listDiamond.Count,gridDiamondSpawn, gridDiamondSpawn.cellSize.x, 1550);
            PerfectSize(listClub.Count,gridClubSpawn, gridClubSpawn.cellSize.x, 1550);
            PerfectSize(listSpade.Count,gridSpadeSpawn, gridSpadeSpawn.cellSize.x, 1550);
            PerfectSize(listHeart.Count,gridHeartSpawn, gridHeartSpawn.cellSize.x, 1550);

        }

        private void PerfectSize(int count,GridLayoutGroup gridLayoutGroup, float cardWidth, float maxWith )
        {
            int numberCards = count;
            float totalCardWidth = numberCards * cardWidth;
            float maxSpacing = maxWith - totalCardWidth;
            float spacing = maxSpacing / (numberCards - 1);
            gridLayoutGroup.spacing = new Vector2(spacing, 0);
        }
        

        private void OnShowStatisticalRankCard()
        {
            GameObjectUtils.Instance.ClearAllChild(transRankCardSpawn.gameObject);
            foreach (RankCard rankCard in Enum.GetValues(typeof(RankCard)))
            {
                if (rankCard != RankCard.None)
                {
                    GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transRankCardSpawn, objPrefabRankCard);
                    var script = obj.GetComponent<CountBoxVo>();
                    script.SetData(rankCard);
                    obj.SetActive(true);
                }
            }
        }

        private void OnShowStatisticalSuitCard()
        {
            GameObjectUtils.Instance.ClearAllChild(transformSpadeCount.gameObject);
            GameObjectUtils.Instance.ClearAllChild(transformHeartCount.gameObject);
            GameObjectUtils.Instance.ClearAllChild(transformClubCount.gameObject);
            GameObjectUtils.Instance.ClearAllChild(transformDiamondCount.gameObject);
            var listDeck = PlayCardController.Instance.ListCardInDeck;
            foreach (RankCard rankCard in Enum.GetValues(typeof(RankCard)))
            {
                var countRank = listDeck.Where(x=>x.PokerRank == rankCard).ToList();
                var countSuit = countRank.Count(x => x.PokerSuit is SuitCard.Spade);
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transformSpadeCount, objCountSuitByRank);
                var text = obj.GetComponent<TextMeshProUGUI>();
                text.text = countSuit.ToString();
                text.enabled = true;
                obj.SetActive(true);
            }
            foreach (RankCard rankCard in Enum.GetValues(typeof(RankCard)))
            {
                var countRank = listDeck.Where(x=>x.PokerRank == rankCard).ToList();
                var countSuit = countRank.Count(x => x.PokerSuit is SuitCard.Heart);
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transformHeartCount, objCountSuitByRank);
                var text = obj.GetComponent<TextMeshProUGUI>();
                text.text = countSuit.ToString();
                text.enabled = true;
                obj.SetActive(true);
            }
            foreach (RankCard rankCard in Enum.GetValues(typeof(RankCard)))
            {
                var countRank = listDeck.Where(x=>x.PokerRank == rankCard).ToList();
                var countSuit = countRank.Count(x => x.PokerSuit is SuitCard.Club);
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transformClubCount, objCountSuitByRank);
                var text = obj.GetComponent<TextMeshProUGUI>();
                text.text = countSuit.ToString();
                text.enabled = true;
                obj.SetActive(true);
            }
            foreach (RankCard rankCard in Enum.GetValues(typeof(RankCard)))
            {
                var countRank = listDeck.Where(x=>x.PokerRank == rankCard).ToList();
                var countSuit = countRank.Count(x => x.PokerSuit is SuitCard.Diamond);
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transformDiamondCount, objCountSuitByRank);
                var text = obj.GetComponent<TextMeshProUGUI>();
                text.text = countSuit.ToString();
                text.enabled = true;
                obj.SetActive(true);
            }
        }
    }
}
