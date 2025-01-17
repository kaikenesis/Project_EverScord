using UnityEngine;

namespace EverScord.Character
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private TestPlayerControl player;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Animator anim;
        [SerializeField] private Transform shootTransform;
        [SerializeField] private AnimationClip shootClip, shootEndClip, adjustShootClip, idleClip;
        [SerializeField] private float coolDown;

        private float elapsedTime = 0f;
        private int adjustLayer;
        private bool isShooting = false;
        private bool isCooldown => elapsedTime < coolDown;

        void Start()
        {
            adjustLayer = anim.GetLayerIndex("AdjustMask");
            anim.SetLayerWeight(adjustLayer, 0f);
        }

        void Update()
        {
            Timer();
            Shoot();
        }

        private void Timer()
        {
            elapsedTime += Time.deltaTime;

            if (isShooting && elapsedTime - coolDown > 2f)
            {
                isShooting = false;
                AdjustPosture(false);
                anim.Play(shootEndClip.name, -1, 0f);
            }
        }

        public void ResetTimer()
        {
            elapsedTime = coolDown;
        }

        private void Shoot()
        {
            if (isCooldown || !Input.GetMouseButton(0))
                return;
            
            elapsedTime = 0f;
            isShooting = true;

            // GameObject bullet = PoolManager.GetObject(bulletPrefab.name);
            // bullet.transform.position = shootTransform.position;

            AdjustPosture(true);
            anim.Play(shootClip.name, -1, 0f);
        }

        private void AdjustPosture(bool shouldAdjust)
        {
            float layerWeight = anim.GetLayerWeight(adjustLayer);

            if (shouldAdjust && layerWeight == 0f)
            {
                anim.SetLayerWeight(adjustLayer, 1f);
                anim.Play(adjustShootClip.name, -1, 0f);
            }
            else if (!shouldAdjust && layerWeight > 0)
            {
                anim.SetLayerWeight(adjustLayer, 0f);
                anim.Play("AdjustReset", -1, 0f);
            }
        }
    }
}
