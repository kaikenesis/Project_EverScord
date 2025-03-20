using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EverScord.Effects;
using UnityEngine.AddressableAssets;

namespace EverScord.Weapons
{
    public class BulletControl : MonoBehaviour
    {
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private int hitEffectCount;
        [SerializeField] private List<AssetReference> fleshHitSounds;
        [SerializeField] private List<AssetReference> metalHitSounds;
        [SerializeField] private List<AssetReference> barrierHitSounds;
        [SerializeField] private List<AssetReference> defaultHitSounds;

        private PhotonView photonView;
        private LinkedList<Bullet> myBullets, otherBullets;
        private IDictionary<int, Bullet> bulletDict;
        private static int nextBulletID = -1;

        void Awake()
        {
            myBullets = new();
            otherBullets = new();
            bulletDict = new Dictionary<int, Bullet>();
            photonView = GetComponent<PhotonView>();

            GameManager.Instance.InitControl(this);
        }

        void Update()
        {
            UpdateBullets(Time.deltaTime, BulletOwner.MINE);
            UpdateBullets(Time.deltaTime, BulletOwner.OTHER);
        }

        public void AddBullet(Bullet bullet, BulletOwner type)
        {
            bulletDict[bullet.BulletID] = bullet;
            
            switch (type)
            {
                case BulletOwner.MINE:
                    myBullets.AddLast(bullet);
                    break;

                case BulletOwner.OTHER:
                    otherBullets.AddLast(bullet);
                    break;

                default:
                    break;
            }
        }

        public void UpdateBullets(float deltaTime, BulletOwner type)
        {
            LinkedList<Bullet> bullets = myBullets;

            if (type == BulletOwner.OTHER)
                bullets = otherBullets;

            if (bullets.Count == 0)
                return;

            LinkedListNode<Bullet> currentNode = bullets.First;

            while (currentNode != null)
            {
                LinkedListNode<Bullet> nextNode = currentNode.Next;

                Bullet bullet = currentNode.Value;
                Weapon weapon = GameManager.Instance.PlayerDict[bullet.ViewID].PlayerWeapon;

                Vector3 currentPosition = bullet.GetPosition();
                bullet.SetLifetime(bullet.Lifetime + deltaTime);
                Vector3 nextPosition    = bullet.GetPosition();

                if (bullet.ShouldBeDestroyed(weapon.WeaponRange))
                {
                    bullet.SetIsDestroyed(true);
                    bullets.Remove(currentNode);

                    ResourceManager.Instance.ReturnToPool(bullet, AssetReferenceManager.Bullet_ID);

                    currentNode = nextNode;

                    if (type == BulletOwner.MINE && PhotonNetwork.IsConnected)
                    {
                        photonView.RPC(nameof(SyncDestroyBullet), RpcTarget.Others, bullet.BulletID);
                        photonView.RPC(nameof(SyncBulletTracer),  RpcTarget.Others, bullet.BulletID, bullet.TracerEffect.transform.position);
                    }

                    continue;
                }

                if (type == BulletOwner.MINE)
                    bullet.CheckCollision(weapon, currentPosition, nextPosition);
                else
                    bullet.SetTracerEffectPosition(nextPosition);

                currentNode = nextNode;
            }
        }

        public static int GetNextBulletID()
        {
            return ++nextBulletID;
        }

        public void BulletHitEffect(Vector3 hitPosition, Vector3 hitDirection)
        {
            hitEffect.transform.position = hitPosition;
            hitEffect.transform.forward  = hitDirection;
            hitEffect.Emit(hitEffectCount);

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncBulletHitEffect), RpcTarget.Others, hitPosition, hitDirection);
        }

        public void PlayBulletSound(IEnemy enemy = null)
        {
            if (!PhotonNetwork.IsConnected)
                return;

            BodyType bodyType = BodyType.DEFAULT;

            if (enemy != null)
                bodyType = enemy.EnemyBodyType;
            
            photonView.RPC(nameof(SyncBulletSound), RpcTarget.All, (int)bodyType);
        }

        public void PlayRandomSound(List<AssetReference> targetSounds)
        {
            int randomIndex = Random.Range(0, targetSounds.Count);
            SoundManager.Instance.PlaySound(targetSounds[randomIndex].AssetGUID);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        [PunRPC]
        private void SyncDestroyBullet(int bulletID)
        {
            bulletDict[bulletID].SetIsDestroyed(true);
        }

        [PunRPC]
        private void SyncBulletHitEffect(Vector3 hitPosition, Vector3 hitDirection)
        {
            hitEffect.transform.position = hitPosition;
            hitEffect.transform.forward  = hitDirection;
            hitEffect.Emit(hitEffectCount);
        }

        [PunRPC]
        private void SyncBulletSound(int bodyType)
        {
            List<AssetReference> targetSounds = null;

            switch ((BodyType)bodyType)
            {
                // On flesh hit, include default sounds for variety
                case BodyType.FLESH:
                {
                    int randomIndex = Random.Range(0, 101);
                    targetSounds = (randomIndex < 30) ? fleshHitSounds : defaultHitSounds;
                    break;
                }

                case BodyType.METAL:
                    targetSounds = metalHitSounds;
                    break;

                case BodyType.BARRIER:
                    targetSounds = barrierHitSounds;
                    break;

                default:
                    targetSounds = defaultHitSounds;
                    break;
            }

            if (targetSounds != null)
                PlayRandomSound(targetSounds);
        }

        [PunRPC]
        private void SyncBulletTracer(int bulletID, Vector3 position)
        {
            bulletDict[bulletID].SetTracerEffectPosition(position);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
    }

    public enum BulletOwner
    {
        MINE,
        OTHER
    }
}

public enum BodyType
{
    DEFAULT,
    FLESH,
    METAL,
    ROCK,
    BARRIER
}