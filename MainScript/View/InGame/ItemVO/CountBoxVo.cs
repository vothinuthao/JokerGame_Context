using Core.Entity;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace InGame
{
    public class CountBoxVo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textRankPoker;
        [SerializeField] private TextMeshProUGUI textCountPoker;
        [SerializeField] private Image imgBg;
        [SerializeField] private Image imgPanel;
        
        [SerializeField] private Sprite spriteGrey;
        [SerializeField] private Sprite spriteRed;
        private RankCard _rank;
        public void SetData(RankCard rank)
        {
            _rank = rank;
            var listDeck = PlayCardController.Instance.ListCardInDeck;
            
            if (_rank == RankCard.Ace)
            {
                textRankPoker.text = "A";
            }
            else if (_rank == RankCard.King)
            {
                textRankPoker.text = "K";
                imgBg.sprite = spriteRed;
            }
            else if (_rank == RankCard.Queen)
            {
                textRankPoker.text = "Q";
                imgBg.sprite = spriteRed;
            }
            else if (_rank == RankCard.Jack)
            {
                textRankPoker.text = "J";
                imgBg.sprite = spriteRed;
            }
            else
            {
                textRankPoker.text = ((int)rank + 1).ToString();
                imgBg.sprite = spriteGrey;
            }
            var count = listDeck.Count(x => x.PokerRank == rank);
            textCountPoker.text = count.ToString();
            imgBg.enabled = true;
            textCountPoker.enabled = true;
            textRankPoker.enabled = true;
            imgPanel.enabled = true;
        }
    }
}
