using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Frameworks.Utils;
using UnityEngine;

namespace Visual.Cards
{
    public class HorizontalCardHolder : MonoBehaviour
    {

        [SerializeField] private CardVisual selectedCard;
        [SerializeReference] private RectTransform rect;

        [Header("Spawn Settings")]
        [SerializeField] private int cardsToSpawn = 7;
        public List<CardVisual> cards;

        bool isCrossing = false;
        [SerializeField] private bool tweenCardReturn = true;

        void Start()
        {
            rect = GetComponent<RectTransform>();

            int cardCount = 0;

            foreach (CardVisual card in cards)
            {
                card.BeginDragEvent.AddListener(BeginDrag);
                card.EndDragEvent.AddListener(EndDrag);
                card.name = cardCount.ToString();
                cardCount++;
            }

            StartCoroutine(Frame());

            IEnumerator Frame()
            {
                yield return new WaitForSecondsRealtime(.1f);
            }
        }
        private void BeginDrag(CardVisual card)
        {
            selectedCard = card;
        }
        void EndDrag(CardVisual card)
        {
            if (selectedCard == null)
                return;

            selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0,selectedCard.selectionOffset,0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

            rect.sizeDelta += Vector2.right;
            rect.sizeDelta -= Vector2.right;

            selectedCard = null;

        }
        void Update()
        {
            if (!selectedCard)
                return;
            if (isCrossing)
                return;
            for (int i = 0; i < cards.Count; i++)
            {
                if (selectedCard.transform.position.x > cards[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                    {
                        Swap(i);
                        break;
                    }
                }
                if (selectedCard.transform.position.x < cards[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                    {
                        Swap(i);
                        break;
                    }
                }
            }
        }
        void Swap(int index)
        {
            isCrossing = true;
            Transform focusedParent = selectedCard.transform.parent;
            Transform crossedParent = cards[index].transform.parent;
            cards[index].transform.SetParent(focusedParent);
            cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
            selectedCard.transform.SetParent(crossedParent);
            isCrossing = false;

            var listCardOnHand = PlayCardController.Instance.ListCardOnHand;
            listCardOnHand.Swap(selectedCard.ParentIndex(), index);
        }
    }
}
