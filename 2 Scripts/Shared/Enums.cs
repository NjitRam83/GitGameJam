using System.Collections.Generic;

namespace BackpackSurvivors.Shared
{
    public class Enums
    {
        public enum WavePositionSpawnType
        {
            Random,
            Grouped
        }

        public enum HighlightState
        {
            None,
            Blocked,
            Available,
        }
        public enum ItemRotation
        {
            Rotation0,
            Rotation90,
            Rotation180,
            Rotation270
        }

        public enum RotateDirection
        {
            Left,
            Right
        }
        public enum MovableBackpackElementType
        {
            BackpackExtension,
            Item
        }

        public enum CursorForm
        {
            Default,
            Grabbing,
            CannotDrop
        }
        public enum ItemQuality
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary,
            Mythic
        }
        public enum ItemType
        {
            Weapon,
            Trinket,
            Shield,
            Armor,
            Boots,
            Gloves,
            Amulet,
            Ring
        }

        public enum StatType
        {
            CritChancePercentage,
            CritMultiplier,
            Health,
            DamagePercentage, //% +10%
            SpeedPercentage,
            LuckPercentage,
            CooldownTime,
            DamageReductionPercentage,
            Armor,
            FlatDamage, // 3-6  flatdamage = 2      5-8     algemene damage
            MinDamage,   //3-6  mindamage = 2    5-6        Stabiele damage
            MaxDamage,   //3-6  maxdamage = 2    3-8        riskant maar dikke damage
            PickupRadiusPercentage,
            EnemyCount, 
            WeaponRange,
            Piercing
        }


        public enum StatCalculationType
        {
            HardValue,
            Percent
        }

        public enum DamageType
        {
            Melee,
            Ranged
        }
        public enum StatTarget
        {
            Player,
            Enemy
        }

        public enum WeaponCategory
        {
        }
        public enum ScalingType
        {
            // scale with luck example
        }


        public enum RangeType
        {
            Melee,
            Ranged
        }

        public enum MeleeAttackType
        {
            Thrust,
            Sweep
        }

        public enum EnemyMovementType
        {
            ChasePlayer,
            RunAwayFromPlayer,
            RunAwayFromPlayerIfTooClose,
            MoveAroundPlayer,
            MoveAroundEnemies,
            DoesNotMove,
            MovesRandomly
        }

        public enum PickupType
        {
        }
        public enum BackpackState
        {
            Init,
            FirstLoad,
            Reward,
            Shop,
            NextWave,
            Hidden
        }

        public enum CharacterType
        { 
            Unknown,
            Player,
            Enemy
        }

        public enum AttackType
        { 
            AttackClosestEnemy,
            AttackNorth,
            AttackEast,
            AttackSouth,
            AttackWest,
            AttackRandomDirection,
            AttackCursor,
            AttackRandomDirectionEachAttack
        }

        public enum AudioType
        { 
            Music,
            Ambiance,
            SFX
        }

        public enum GamePlayType
        { 
            Normal,
            DailyChallenge
        }
    }
}