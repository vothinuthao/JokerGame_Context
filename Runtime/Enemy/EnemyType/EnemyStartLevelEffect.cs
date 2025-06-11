using System.Threading.Tasks;
using InGame;
using Runtime.Manager;

namespace Enemy.EnemyType
{
    public class EnemyStartLevelEffect : EnemyCardLogic
    {
        private EnemyCardData.TriggerPhaseEnemyEnum _effectEnum = EnemyCardData.TriggerPhaseEnemyEnum.WhenStartLevel;
        private EnemySo _data;
        
        public override EnemyCardData.TriggerPhaseEnemyEnum GetProductType()
        {
            return EnemyCardData.TriggerPhaseEnemyEnum.WhenStartLevel;
        }

        public override Task EffectCard(EnemyCardVo enemyCardVo, PokerCardVo pokerCardVo = null)
        {
            _data = enemyCardVo.EnemySO;
            if (_data.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.CardSlicer)
            {
                if(enemyCardVo.Property.Health > 0)
                    PlayCardController.Instance.RandomRemoveCard((int)_data.value);
            }

            return Task.CompletedTask;
        }  
    }
}