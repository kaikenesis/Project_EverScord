using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AddressableAssets;

namespace EverScord.Weapons
{
    public class BulletControl : MonoBehaviour
    {
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private int hitEffectCount;
        [SerializeField] private List<AssetReferenceGameObject> assetReferenceList;

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

            GameManager.Instance.SetBulletControl(this);

            InitPools();
        }

        private async void InitPools()
        {
            foreach (AssetReference reference in assetReferenceList)
                await ResourceManager.Instance.CreatePool(reference.AssetGUID);
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
                    BulletHitEffect(bullet);

                    ResourceManager.instance.ReturnToPool(bullet.gameObject, weapon.BulletAssetReference.AssetGUID);

                    currentNode = nextNode;

                    if (PhotonNetwork.IsConnected)
                    {
                        photonView.RPC(
                            "SyncDestroyBullet",
                            RpcTarget.Others,
                            bullet.BulletID,
                            bullet.BulletHitPosition != null,
                            bullet.BulletHitPosition ?? Vector3.zero,
                            bullet.BulletHitDirection
                        );
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

        private void BulletHitEffect(Bullet bullet)
        {
            if (bullet.BulletHitPosition == null)
                return;
            
            hitEffect.transform.position = (Vector3)bullet.BulletHitPosition;
            hitEffect.transform.forward  = bullet.BulletHitDirection;
            hitEffect.Emit(hitEffectCount);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        [PunRPC]
        private void SyncDestroyBullet(int bulletID, bool isHit, Vector3 hitPosition, Vector3 hitDirection)
        {
            bulletDict[bulletID].SetIsDestroyed(true);

            if (isHit)
                bulletDict[bulletID].SetBulletHitInfo(hitPosition, hitDirection);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
    }

    public enum BulletOwner
    {
        MINE,
        OTHER
    }
}
