using System;
using Core.Entity;
using UnityEngine.Serialization;

namespace Entity
{
    [Serializable]
    public abstract class BaseCardEntity
    {
        public EnhancementsModifyCardEnum pokerEnhancement;
        public EditionsModifyCardEnum pokerEdition;
        public SealsModifyCardEnum pokerSeal;
        public bool isDisableCard = false;
    }
}
