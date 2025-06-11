using Frameworks.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager
{
    public class DataDurationManager : ManualSingletonMono<DataDurationManager>
    {
        [FoldoutGroup("Base Card Point")]
        [SerializeField] private float durationBaseCardPerPlay;
        [FoldoutGroup("Base Card Point")]
        [SerializeField] private float durationBaseCardPerChip;
        [FoldoutGroup("Base Card Point")]
        [SerializeField] private float durationBaseCardPerMult;
        [FoldoutGroup("Base Card Point")]
        [SerializeField] private float durationBaseCardPerDollar;
        [FoldoutGroup("Joker Card Point")]
        [SerializeField] private float durationJokerCardChip;
        [FoldoutGroup("Joker Card Point")]
        [SerializeField] private float durationJokerCardMult;
        
        [FoldoutGroup("Enemy Point")]
        [SerializeField] private float durationEnemyPlayPoint;
        
        public float DurationBaseCardPerPlay => durationBaseCardPerPlay;
        public float DurationBaseCardPerChip => durationBaseCardPerChip;
        public float DurationBaseCardPerMult => durationBaseCardPerMult;
        public float DurationBaseCardPerDollar => durationBaseCardPerDollar;
        
        
        public float DurationJokerCardChip => durationJokerCardChip;
        public float DurationJokerCardMult => durationJokerCardMult;
        
        
        public float DurationEnemyPlayPoint => durationEnemyPlayPoint;

        public int ConvertFloatTimeToIntDurationChip()
        {
            var getTime = durationBaseCardPerChip * 100f;
            return (int)getTime;
        }
        
        
        
    }
}
