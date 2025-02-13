using UnityEngine;

namespace EverScord
{
    public class MiddleBossBTRunner : BTree
    {
        [SerializeField] private MiddleBossData data;
        [SerializeField] private MiddleBossController controller;

        protected override void SetupTree()
        {
            //_ = ResourceManager.Instance.CreatePool("BossProjectile");
            root.Init();
            root.SetValue("Data", data);
            root.SetValue("Owner", controller);
            root.Setup(gameObject);
        }
    }
}

