using System;
using System.Collections.Generic;
using Core.Entity;
using Core.Utils;
using UnityEngine;
using UnityEngine.U2D;

namespace Entity
{
    [Serializable]
    public class SpritesSuit
    {
        public SuitCard SuitName;
        public SpriteAtlas SpriteAtlas;
    }
    public class SpritesManager : ManualSingletonMono<SpritesManager>
    {
        [SerializeField]
        private List<Sprite> _listSpriteJoker = new List<Sprite>();
        [SerializeField]
        private List<Sprite> _listSpriteBoosterPack = new List<Sprite>();
        [SerializeField]
        private List<Sprite> _listSpritePlanetPack = new List<Sprite>();
        [SerializeField]
        private List<Sprite> _listSpriteSpellCard = new List<Sprite>();
        
        [SerializeField]
        private List<Sprite> _listSpriteCommon = new List<Sprite>();

        
        [SerializeField] private SpriteAtlas listSpritesPoker;
        
        private string _boosterPackName = "boosters";
        private string _consumablePackName = "Tarots";
        private string _jokerName = "Jokers";
        
        public Sprite GetSpriteBoosterPackById(int index)
        {
            if (_listSpriteBoosterPack.Count > 0)
            {
                string name = _boosterPackName + "_" + index;
                foreach (var sprite in _listSpriteBoosterPack)
                {
                    if (sprite.name == name)
                        return sprite;
                }
            }
            Debug.LogError("No sprites found in the specified sprite sheet.");
            return null;
        }
        public Sprite GetSpritePlanetPackById(int index)
        {
            if (_listSpritePlanetPack.Count > 0)
            {
                string name = _consumablePackName + "_" + index;
                foreach (var sprite in _listSpritePlanetPack)
                {
                    if (sprite.name == name)
                        return sprite;
                }
            }
            Debug.LogError("No sprites found in the specified sprite sheet.");
            return null;
        }
        public Sprite GetSpriteSpellCardById(int index)
        {
            if (_listSpriteSpellCard.Count > 0)
            {
                string name = _consumablePackName + "_" + index;
                foreach (var sprite in _listSpriteSpellCard)
                {
                    if (sprite.name == name)
                        return sprite;
                }
            }
            Debug.LogError("No sprites found in the specified sprite sheet.");
            return null;
        }
        public Sprite GetSpritesByRankAndSuit(RankCard rank, SuitCard suit)
        {
            var stringName = $"Card_{suit}_{(int)rank}";
            return listSpritesPoker.GetSprite(stringName);
        }
        
        public Sprite GetSpritesCommonByName(string nameSprite)
        {
            foreach (var sprite in _listSpriteCommon)
            {
                if (sprite.name == nameSprite)
                    return sprite;
            }
            return null;
        }
    }
}