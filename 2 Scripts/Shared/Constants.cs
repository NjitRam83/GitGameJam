using UnityEngine;

namespace BackpackSurvivors.Shared
{
    public class Constants
    {
        public const float DefaultPlayerMoveSpeed = 6f;

        public class Scenes
        {
            public const string MainMenuScene = "1. MainMenuScene";
            public const string DailyChallengesScene = "1.2 Daily Challenges Scene";
            public const string ExplainationScene = "1.5. ExplanationScene";
            public const string StoryScene = "1.9. StoryScene";
            public const string MountainScalingScene = "2. MountainScene";
            public const string GrassLevelScene = "3.1. Grass Level";
            public const string CemeteryLevelScene = "3.2. Cemetery Level";
            public const string SewerLevelScene = "3.3. Sewer Level";
            public const string CaveLevelScene = "3.4. Cave Level";
            public const string BossLevelScene = "3.5. Boss Level";
            public const string CreditsScene = "10. CreditsScene";
        }

        public class PlayerPrefsVariables
        {
            public const string PlayerId = "PlayerId";
        }

        public class LootLocker
        {
            public class Leaderboards
            {
                public const string MaxLevelFinishedKey = "max_level_finished";
                public class DailyChallenges
                {
                    public const string DailyChallengeDay1Key = "daily_challenge_day_1";
                    public const string DailyChallengeDay2Key = "daily_challenge_day_2";
                    public const string DailyChallengeDay3Key = "daily_challenge_day_3";
                    public const string DailyChallengeDay4Key = "daily_challenge_day_4";
                    public const string DailyChallengeDay5Key = "daily_challenge_day_5";
                    public const string DailyChallengeDay6Key = "daily_challenge_day_6";
                    public const string DailyChallengeDay7Key = "daily_challenge_day_7";
                    public const string DailyChallengeDay8Key = "daily_challenge_day_8";
                    public const string DailyChallengeDay9Key = "daily_challenge_day_9";
                    public const string DailyChallengeDay10Key = "daily_challenge_day_10";
                    public const string DailyChallengeDay11Key = "daily_challenge_day_11";
                    public const string DailyChallengeDay12Key = "daily_challenge_day_12";
                    public const string DailyChallengeDay13Key = "daily_challenge_day_13";
                    public const string DailyChallengeDay14Key = "daily_challenge_day_14";
                    public const string DailyChallengeDay15Key = "daily_challenge_day_15";
                    public const string DailyChallengeDay16Key = "daily_challenge_day_16";
                    public const string DailyChallengeDay17Key = "daily_challenge_day_17";
                    public const string DailyChallengeDay18Key = "daily_challenge_day_18";
                    public const string DailyChallengeDay19Key = "daily_challenge_day_19";
                    public const string DailyChallengeDay20Key = "daily_challenge_day_20";
                    public const string DailyChallengeDay21Key = "daily_challenge_day_21";
                    public const string DailyChallengeDay22Key = "daily_challenge_day_22";
                    public const string DailyChallengeDay23Key = "daily_challenge_day_23";
                    public const string DailyChallengeDay24Key = "daily_challenge_day_24";
                    public const string DailyChallengeDay25Key = "daily_challenge_day_25";
                    public const string DailyChallengeDay26Key = "daily_challenge_day_26";
                    public const string DailyChallengeDay27Key = "daily_challenge_day_27";
                    public const string DailyChallengeDay28Key = "daily_challenge_day_28";
                    public const string DailyChallengeDay29Key = "daily_challenge_day_29";
                    public const string DailyChallengeDay30Key = "daily_challenge_day_30";
                    public const string DailyChallengeDay31Key = "daily_challenge_day_31";
                }
            }
        }

        public class Layers
        {
            public const string NoColission = "NoColission";
        }

        public class Colors
        {
            public static Color PositiveEffectColor = Color.green;
            public static Color NegativeEffectColor = Color.red;
            public static Color ShopItemStatTitleColor = new Color(248, 239, 186);
            public static Color ShopItemCategoryColor = new Color(248, 239, 186);
            public static Color WeaponBaseDamageColor = new Color(91, 91, 91);
            public static Color PlayerHealingColor = new Color(0, 255, 33);
            public static Color PlayerDamagedColor = new Color(127, 0, 55);
            public static Color PlayerDamagedByCritColor = new Color(255, 48, 234);
            public static Color EnemyDamagedColor = new Color(255, 255, 255);
            public static Color EnemyDamagedByCritColor = new Color(255, 116, 0);

            public class HexStrings
            {
                public const string ShopItemStatTitleColor = "F8EFBA";
                public const string RarityCommonColor = "999999";
                public const string RarityUncommonColor = "2DFF49";
                public const string RarityRareColor = "1696FF";
                public const string RarityEpicColor = "E81CFF";
                public const string RarityLegendaryColor = "FF743D";
            }
        }

        public class Sprites
        {
            public class PrimaryStats
            {
                public const string Level = "Level";
                public const string MaxHp = "MaxHp";
                public const string HpRegeneration = "HpRegeneration";
                public const string Lifesteal = "Lifesteal";
                public const string Damage = "Damage";
                public const string MeleeDamage = "MeleeDamage";
                public const string RangedDamage = "RangedDamage";
                public const string ElementalDamage = "ElementalDamage";
                public const string AttackSpeed = "AttackSpeed";
                public const string CritChance = "CritChance";
                public const string Engineering = "Engineering";
                public const string Range = "Range";
                public const string Armor = "Armor";
                public const string DodgeChance = "DodgeChance";
                public const string Speed = "Speed";
                public const string Luck = "Luck";
                public const string Harvesting = "Harvesting";
            }
        }
    }
}
