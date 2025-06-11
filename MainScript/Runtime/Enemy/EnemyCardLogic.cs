using System.Threading.Tasks;
using Core.Pattern;
using InGame;

namespace Enemy
{
    public class EnemyCardLogic : ITypeOnFactory<EnemyCardData.TriggerPhaseEnemyEnum>
    {
        public virtual EnemyCardData.TriggerPhaseEnemyEnum GetProductType() => EnemyCardData.TriggerPhaseEnemyEnum.None;
        public virtual Task EffectCard(EnemyCardVo enemyCardVO, PokerCardVo pokerCardVo = null)
        {
            return Task.CompletedTask;
        }
        
    }
    public class EnemyFactory : AFactory<EnemyCardData.TriggerPhaseEnemyEnum, EnemyCardLogic>
    {
        
    }
}