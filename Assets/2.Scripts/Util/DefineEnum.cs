using UnityEngine;

namespace DefineEnum
{
    #region[GamePlay]
    public enum TableName
    {
        MonsterInfoList,
        MusicList
    }

    public enum HeartSatus
    {
        Empty,
        Full
    }

    public enum LookDir
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        Up2,
        Down2,
        Left2,
        Right2,

        count
    }

    public enum Monsters
    {
        Slime,
        Bat,
        Skeleton,
        Golem,
        RedDragon,
        Banshee,
        DireBat, 

        Count
    }

    public enum GameScene
    {
        LoginScene,
        LobbyScene,
        GamePlayScene
    }

    #endregion[GamePlay]

    #region[Item]
    public enum MonsterPriority
    {
        Slime = 1,
        Bat, 
        Skeleton,
        Golem,
        DireBat,
        Banshee,
        RedDragon,

        Count
    }

    public enum WeaponName
    {
        DaggerN,
        DaggerB,
        DaggerT,
        DaggerO1,
        DaggerO2,
        DaggerO3,

        SwordN,
        SwordB,
        SwordT,
        SwordO1,
        SwordO2,
        SwordO3,

        SpearN,
        SpearB,
        SpearT,
        SpearO1,
        SpearO2,
        SpearO3,
    }

    public enum Upgrade
    {
        StrUp,
        HealthUp
    }

    public enum Food
    {
        Apple,
        Cheese,
        Chicken
    }
    #endregion[Item]

    #region[Music]
    public enum BGMName
    {
        Disco_Descent,
        Crypt_FamilyJules_Remix,
        Crypteque,
        March_of_the_Profane,

        Count
    }

    public enum LoopName
    {
        Disco_Descent_loop,
        Crypt_FamilyJules_Remix_loop,
        Crypteque_loop,
        March_of_the_Profane_loop,
        Rhythmortis_lobby,

        Count
    }

    public enum ShopkeeperName
    {
        Disco_Descent_shopkeeper_loop,
        Crypt_FamilyJules_Remix_shopkeeper_loop,
        Crypteque_shopkeeper_loop,
        March_of_the_Profane_shopkeeper_loop,

        Count
    }

    public enum SFXName
    {
        Cadence_death_01,
        Cadence_death_02,
        Cadence_death_03,

        Cadence_hurt_01,
        Cadence_hurt_02,
        Cadence_hurt_03,
        Cadence_hurt_04,
        Cadence_hurt_05,
        Cadence_hurt_06,

        Cadence_Attack_Combo_01,
        Cadence_Attack_Combo_02,
        Cadence_Attack_Combo_03,
        Cadence_Attack_Combo_04,

        Cadence_yeah_01,
        Cadence_yeah_02,
        Cadence_yeah_03,
        Cadence_yeah_04,
        Cadence_yeah_05,

        Cadence_teleport_01,
        Cadence_teleport_02,
        Cadence_teleport_03,
        Cadence_teleport_04,
        Cadence_teleport_05,

        Banshee_attack,
        Banshee_cry,
        Banshee_death,
        Banshee_hurt_01,
        Banshee_hurt_02,
        Banshee_hurt_03,

        Bat_attack,
        Bat_death,
        Bat_hit,
        Bat_minibpss_hit,

        Dragon_attack_fire,
        Dragon_attack_melee,
        Dragon_attack_prefire,
        Dragon_cry,
        Dragon_death,
        Dragon_hurt_01,
        Dragon_hurt_02,
        Dragon_hurt_03,
        Dragon_walk_01,
        Dragon_walk_02,
        Dragon_walk_03,

        Direbat_attack,
        Direbat_cry,
        Direbat_death,
        Direbat_hit_01,
        Direbat_hit_02,
        Direbat_hit_03,

        Golemstone_attack,
        Golemstone_death,
        Golemstone_hurt_01,
        Golemstone_hurt_02,
        Golemstone_hurt_03,
        Golemstone_move_01,
        Golemstone_move_02,
        Golemstone_move_03,

        Skel_attack_melee,
        Skel_death,
        Skel_hurt_01,
        Skel_hurt_02,
        Skel_hurt_03,

        Slime_attack,
        Slime_death_01,
        Slime_death_02,
        Slime_death_03,
        Slime_hurt_01,
        Slime_hurt_02,
        Slime_hurt_03,

        sfx_player_death_ST,
        sfx_player_hit_ST,
        sfx_secretfound,
        sfx_ui_back,
        sfx_ui_select_down,
        sfx_ui_select_up,
        sfx_pickup_gold_01,
        sfx_pickup_gold_02,
        sfx_pickup_gold_03,
        sfx_pickup_purchase,
        sfx_chain_break_ST,
        sfx_chain_groove_ST,
        sfx_error_ST,
        sfx_item_food,

        Count
    }
    #endregion[Music]

    #region[Unlockable]

    public enum UnlockItems
    {
        Cheese,
        Sword,
        DaggerBlood,
        DaggerTitanium,
        Chicken,
        SwordBlood,
        SwordTitanium,
        DaggerObsidium,
        SwordObsidium,

        Count
    }

    #endregion[Unlockable]
}
