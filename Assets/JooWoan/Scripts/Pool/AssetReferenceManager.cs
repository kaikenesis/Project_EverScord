using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord.Effects
{
    public class AssetReferenceManager : Singleton<AssetReferenceManager>
    {
        [field: SerializeField] public AssetReference Bullet              { get; private set; }
        [field: SerializeField] public AssetReference BulletSmoke         { get; private set; }
        [field: SerializeField] public AssetReference MeshTrailDummy      { get; private set; }
        [field: SerializeField] public AssetReference ReviveCircle        { get; private set; }
        [field: SerializeField] public AssetReference TrajectoryLineMat   { get; private set; }
        [field: SerializeField] public AssetReference BlinkWhiteMat       { get; private set; }
        [field: SerializeField] public AssetReference BloodMat            { get; private set; }
        [field: SerializeField] public AssetReference CrosshairIcon       { get; private set; }

        public static string Bullet_ID              => Instance.Bullet.AssetGUID;
        public static string BulletSmoke_ID         => Instance.BulletSmoke.AssetGUID;
        public static string MeshTrailDummy_ID      => Instance.MeshTrailDummy.AssetGUID;
        public static string ReviveCircle_ID        => Instance.ReviveCircle.AssetGUID;
        public static string TrajectoryLineMat_ID   => Instance.TrajectoryLineMat.AssetGUID;
        public static string BlinkWhiteMat_ID       => Instance.BlinkWhiteMat.AssetGUID;
        public static string BloodMat_ID            => Instance.BloodMat.AssetGUID;
        public static string CrosshairIcon_ID       => Instance.CrosshairIcon.AssetGUID;


        // EFFECTS
        [field: SerializeField] public AssetReference HealEffect        { get; private set; }
        [field: SerializeField] public AssetReference DeathEffect       { get; private set; }
        [field: SerializeField] public AssetReference ReviveEffect      { get; private set; }
        [field: SerializeField] public AssetReference HitEffect1        { get; private set; }
        [field: SerializeField] public AssetReference HitEffect2        { get; private set; }

        public static string HealEffect_ID          => Instance.HealEffect.AssetGUID;
        public static string DeathEffect_ID         => Instance.DeathEffect.AssetGUID;
        public static string ReviveEffect_ID        => Instance.ReviveEffect.AssetGUID;
        public static string HitEffect1_ID          => Instance.HitEffect1.AssetGUID;
        public static string HitEffect2_ID          => Instance.HitEffect2.AssetGUID;

    }
}
