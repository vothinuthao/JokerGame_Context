using System.Collections.Generic;
using Core.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Create New Enemy", menuName = "Enemy/Minion", order = 100)]
    public class EnemySo : ScriptableObject
    {
        public uint id;
        public string enemyName;
        public float baseHealth;
        public float multiplierHealth;
        public List<EnemyTagsEnum> enemyTagsEnum;
        public int drawCard;
        public EnemyCardData.TypeEnemyEnum typeEnemy;
        public string description;
        public Sprite imgEnemy;
        public bool active;
        public List<EnemyCardData.EnemyLevelHandModify> listValueHandLevelConfig;
        [FoldoutGroup("Passive of Enemy")]
        public EnemyCardData.PassiveEffectEnemy passiveNameEnemy;
        [FoldoutGroup("Passive of Enemy")]
        public EnemyCardData.TriggerPhaseEnemyEnum triggerPhaseEnemy;
        [FoldoutGroup("Passive of Enemy")]
        public float value;

        [ShowIf("StatsForBoss")] 
        public Sprite iconBoss;
        [ShowIf("StatsForBoss")]
        public string nameRoundBoss = "Round Boss";
        private bool StatsForBoss => typeEnemy == EnemyCardData.TypeEnemyEnum.Boss;
    }
}