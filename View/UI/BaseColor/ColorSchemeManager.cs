using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Entity;
using Frameworks.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI.BaseColor
{
    internal class ColorSchemeManager : ManualSingletonMono<ColorSchemeManager>
    {
        [SerializeField] Color colorMainText = Color.white;
       
        [FoldoutGroup("Score Color")]
        [SerializeField] Color colorHands = Color.blue;
        [FoldoutGroup("Score Color")]
        [SerializeField] Color colorDiscards = Color.red;
        [FoldoutGroup("Score Color")]
        public Color colorMult = Color.red;
        [FoldoutGroup("Score Color")]
        public Color colorChip = Color.blue;
        [FoldoutGroup("Score Color")]
        [SerializeField] Color colorDollar = Color.yellow;
        [FoldoutGroup("Score Color")]
        [SerializeField] Color colorRank = Color.yellow;
        [FoldoutGroup("Score Color")]
        [SerializeField] Color colorSuit = Color.cyan;
        
        [FoldoutGroup("Suit Color")]
        [SerializeField] Color colorDiamond = Color.yellow;
        [FoldoutGroup("Suit Color")]
        [SerializeField] Color colorHeart = Color.yellow;
        [FoldoutGroup("Suit Color")]
        [SerializeField] Color colorSpade = Color.yellow;
        [FoldoutGroup("Suit Color")]
        [SerializeField] Color colorClub = Color.yellow;
        
        [FoldoutGroup("Card Color")]
        [SerializeField] Color colorSpellCard = Color.blue;

        [FoldoutGroup("Symbol Score Color")] [SerializeField]
        private string symbolHand = "%s1Hand";
        [FoldoutGroup("Symbol Score Color")] [SerializeField]
        private string symbolDiscard = "%s1Discard";
        
        [FoldoutGroup("Symbol Text Color")] [SerializeField]
        private string symbolMult = "%s1Mult";
        [FoldoutGroup("Symbol Text Color")] [SerializeField]
        private string symbolChip = "%s1Chip";
        [FoldoutGroup("Symbol Text Color")] [SerializeField]
        private string symbolDollar = "%s1Dollar";
        
        [FoldoutGroup("Symbol Suit Color")] [SerializeField]
        private string symbolDiamond = "%s1Diamond";
        [FoldoutGroup("Symbol Suit Color")] [SerializeField]
        private string symbolHeart = "%s1Heart";
        [FoldoutGroup("Symbol Suit Color")] [SerializeField]
        private string symbolSpade = "%s1Spade";
        [FoldoutGroup("Symbol Suit Color")] [SerializeField]
        private string symbolClub = "%s1Club";
        
        [FoldoutGroup("Symbol Card Color")] [SerializeField]
        private string symbolSpellCard = "%s1Spell";
        
        private Dictionary<string, Color> _dictSymbolColor = new Dictionary<string, Color>();
        public override void Awake()
        {
            base.Awake();
            _dictSymbolColor.Add(symbolMult, colorMult);
            _dictSymbolColor.Add(symbolChip, colorChip);
            _dictSymbolColor.Add(symbolDollar, colorDollar);
            //score
            _dictSymbolColor.Add(symbolHand, colorHands);
            _dictSymbolColor.Add(symbolDiscard, colorDiscards);
            //suit 
            _dictSymbolColor.Add(symbolDiamond, colorDiamond);
            _dictSymbolColor.Add(symbolHeart, colorHeart);
            _dictSymbolColor.Add(symbolSpade, colorSpade);
            _dictSymbolColor.Add(symbolClub, colorClub);
            // card 
            _dictSymbolColor.Add(symbolSpellCard, colorSpellCard);
        }

        public Color GetMainColorByEnum(ColorScoreEnum scoreType)
        {
            Color resultColor = colorMainText;
            switch (scoreType)
            {
                case ColorScoreEnum.None:
                case ColorScoreEnum.HandColor:
                    resultColor = colorHands;
                    break;
                case ColorScoreEnum.DiscardColor:
                    resultColor = colorDiscards;
                    break;
                case ColorScoreEnum.MultColor:
                    resultColor = colorMult;
                    break;
                case ColorScoreEnum.ChipColor:
                    resultColor = colorChip;
                    break;
                case ColorScoreEnum.DollarColor:
                    resultColor = colorDollar;
                    break;
                case ColorScoreEnum.RankColor:
                    resultColor = colorRank;
                    break;
                case ColorScoreEnum.SuitColor:
                    resultColor = colorSuit;
                    break;
            }
            return resultColor;
    }

        public string ConvertColorTextFromSymbol(string originalText)
        {
            if (string.IsNullOrEmpty(originalText)) return "";
            string formattedText = originalText;
            foreach (KeyValuePair<string, Color> entry in _dictSymbolColor)
            {
                string pattern = entry.Key + "(.*?)" + entry.Key;
                string hexColor = ColorUtility.ToHtmlStringRGB(entry.Value);
                originalText = Regex.Replace(originalText, pattern, $"<color=#{hexColor}>$1</color>");
            }
            
            originalText = originalText.Replace("\r", "");  
            return originalText;
        }
        
    }
}
