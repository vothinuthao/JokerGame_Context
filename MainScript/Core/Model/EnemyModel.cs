using System;
using System.Collections.Generic;
using Core.Entity;
using Enemy;
using UnityEngine;

namespace Core.Model
{
    public class EnemyModel
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public float MaxHealth { get; set; }
        public float Health { get; set; }
        public float MultiplierHealth { get; set; }
        public EnemyDifficultyEnum EnemyDifficultyEnum { get; set; }
        public EnemyCardData.TypeEnemyEnum TypeEnemyEnum { get; set; }
        public EnemyTagsEnum EnemyRankEnum { get; set; }
        public int DrawCards{ get; set; }
        public bool IsUsePassive { get; set; }
        public bool Active { get; set; }

        public EnemyCardData.PassiveEffectEnemy PassiveNameEnemy { get; set; }
        public EnemyCardData.TriggerPhaseEnemyEnum TriggerPhaseEnemy { get; set; }
        public List<EnemyCardData.EnemyLevelHandModify> ListValueHandLevelConfig { get; set; }

        public float Value { get; set; }
        public Sprite IconBoss { get; set; }
    }
}