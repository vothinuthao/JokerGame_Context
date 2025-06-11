using System.Collections.Generic;
using Core.Entity;
using Core.Observer;

namespace Player
{
    public class ShopDataModel
    {
        public string Uid { get; set; }
        public List<int> JokerOnShop { get; set; } = new List<int>();
        public List<int> BoosterPackOnShop { get; set; } = new List<int>();
        
        
        public ShopDataModel()
        {
        
        }
    }
    
    
}