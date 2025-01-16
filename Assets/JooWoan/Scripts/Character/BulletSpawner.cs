using UnityEngine;

namespace EverScord.Character
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Animator shootAnim;
        [SerializeField] private Transform shootTransform;
        [SerializeField] private AnimationClip shootClip;
        [SerializeField] private float coolDown;

        private float elapsedTime = 0f;
        private bool isCooldown => elapsedTime < coolDown;

        private int upperLayer;

        void Start()
        {
            upperLayer = shootAnim.GetLayerIndex("UpperBody");
        }
        
        void Update()
        {
            elapsedTime += Time.deltaTime;

            if (!isCooldown && Input.GetMouseButton(0))
            {
                elapsedTime = 0f;
                Shoot();
            }
        }

        public void ResetShootTimer()
        {
            elapsedTime = coolDown;
        }

        private void Shoot()
        {
            // GameObject bullet = PoolManager.GetObject(bulletPrefab.name);
            // bullet.transform.position = shootTransform.position;
            shootAnim.Play(shootClip.name, upperLayer, 0f);
        }
    }
}
