using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord.Effects
{
    // Register the AssetReference here. If object pooling is needed, register it in PoolLoader as well.
    public class AssetReferenceManager : Singleton<AssetReferenceManager>
    {
        [field: SerializeField] public AssetReference Bullet                    { get; private set; }
        [field: SerializeField] public AssetReference BulletSmoke               { get; private set; }
        [field: SerializeField] public AssetReference MeshTrailDummy            { get; private set; }
        [field: SerializeField] public AssetReference ReviveCircle              { get; private set; }
        [field: SerializeField] public AssetReference TrajectoryLineMat         { get; private set; }
        [field: SerializeField] public AssetReference BlinkWhiteMat             { get; private set; }
        [field: SerializeField] public AssetReference BloodMat                  { get; private set; }
        [field: SerializeField] public AssetReference DissolveMat               { get; private set; }
        [field: SerializeField] public AssetReference CrosshairIcon             { get; private set; }
        [field: SerializeField] public AssetReference HealEffect                { get; private set; }
        [field: SerializeField] public AssetReference DeathEffect               { get; private set; }
        [field: SerializeField] public AssetReference ReviveEffect              { get; private set; }
        [field: SerializeField] public AssetReference ReviveBeam                { get; private set; }
        [field: SerializeField] public AssetReference HitEffect1                { get; private set; }
        [field: SerializeField] public AssetReference HitEffect2                { get; private set; }
        [field: SerializeField] public AssetReference TeleportEffect            { get; private set; }
        [field: SerializeField] public AssetReference StunnedDebuffEffect       { get; private set; }
        [field: SerializeField] public AssetReference StunnedDebuffOut          { get; private set; }
        [field: SerializeField] public AssetReference SpawnMarker               { get; private set; }
        [field: SerializeField] public AssetReference SpawnSmoke                { get; private set; }

        [field: SerializeField] public AssetReference Boss                      { get; private set; }
        [field: SerializeField] public AssetReference BossProjectile            { get; private set; }
        [field: SerializeField] public AssetReference MonsterProjectile         { get; private set; }
        [field: SerializeField] public AssetReference NMM2_Projectile           { get; private set; }
        [field: SerializeField] public AssetReference StoneUp                   { get; private set; }
        [field: SerializeField] public AssetReference MonsterAttack             { get; private set; }
        [field: SerializeField] public AssetReference BossMonsterStoneAttack    { get; private set; }
        [field: SerializeField] public AssetReference P15_Effect                { get; private set; }
        [field: SerializeField] public AssetReference NML2_A1_Effect01          { get; private set; }
        [field: SerializeField] public AssetReference NML2_A2_Effect            { get; private set; }

        public static string Bullet_ID                  => Instance.Bullet.AssetGUID;
        public static string BulletSmoke_ID             => Instance.BulletSmoke.AssetGUID;
        public static string MeshTrailDummy_ID          => Instance.MeshTrailDummy.AssetGUID;
        public static string ReviveCircle_ID            => Instance.ReviveCircle.AssetGUID;
        public static string TrajectoryLineMat_ID       => Instance.TrajectoryLineMat.AssetGUID;
        public static string BlinkWhiteMat_ID           => Instance.BlinkWhiteMat.AssetGUID;
        public static string BloodMat_ID                => Instance.BloodMat.AssetGUID;
        public static string DissolveMat_ID             => Instance.DissolveMat.AssetGUID;
        public static string CrosshairIcon_ID           => Instance.CrosshairIcon.AssetGUID;
        public static string HealEffect_ID              => Instance.HealEffect.AssetGUID;
        public static string DeathEffect_ID             => Instance.DeathEffect.AssetGUID;
        public static string ReviveEffect_ID            => Instance.ReviveEffect.AssetGUID;
        public static string ReviveBeam_ID              => Instance.ReviveBeam.AssetGUID;
        public static string HitEffect1_ID              => Instance.HitEffect1.AssetGUID;
        public static string HitEffect2_ID              => Instance.HitEffect2.AssetGUID;
        public static string TeleportEffect_ID          => Instance.TeleportEffect.AssetGUID;
        public static string StunnedDebuffEffect_ID     => Instance.StunnedDebuffEffect.AssetGUID;
        public static string StunnedDebuffOut_ID        => Instance.StunnedDebuffOut.AssetGUID;
        public static string SpawnMarker_ID             => Instance.SpawnMarker.AssetGUID;
        public static string SpawnSmoke_ID              => Instance.SpawnSmoke.AssetGUID;

        public static string Boss_ID                    => Instance.Boss.AssetGUID;
        public static string BossProjectile_ID          => Instance.BossProjectile.AssetGUID;
        public static string MonsterProjectile_ID       => Instance.MonsterProjectile.AssetGUID;
        public static string NMM2_Projectile_ID         => Instance.NMM2_Projectile.AssetGUID;
        public static string StoneUp_ID                 => Instance.StoneUp.AssetGUID;
        public static string MonsterAttack_ID           => Instance.MonsterAttack.AssetGUID;
        public static string BossMonsterStoneAttack_ID  => Instance.BossMonsterStoneAttack.AssetGUID;
        public static string P15_Effect_ID              => Instance.P15_Effect.AssetGUID;
        public static string NML2_A1_Effect01_ID        => Instance.NML2_A1_Effect01.AssetGUID;
        public static string NML2_A2_Effect_ID          => Instance.NML2_A2_Effect.AssetGUID;
    }
}
