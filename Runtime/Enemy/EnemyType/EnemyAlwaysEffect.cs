using System.Threading.Tasks;
using InGame;

namespace Enemy.EnemyType
{
    public class EnemyAlwaysEffect : EnemyCardLogic
    {
        private EnemyCardData.TriggerPhaseEnemyEnum _effectEnum = EnemyCardData.TriggerPhaseEnemyEnum.Always;
        private EnemySo _data;

        public override EnemyCardData.TriggerPhaseEnemyEnum GetProductType()
        {
            return EnemyCardData.TriggerPhaseEnemyEnum.Always;
        }

        public override async Task EffectCard(EnemyCardVo enemyCardVO, PokerCardVo pokerCardVo = null)
        {
        }  
    }
}