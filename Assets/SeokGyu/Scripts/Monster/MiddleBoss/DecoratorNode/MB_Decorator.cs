using UnityEngine;

namespace EverScord
{
    public abstract class MB_Decorator : BehaviorNode
    {
        protected MiddleBossData m_Data;
        protected MiddleBossController owner;

        public override void Setup(GameObject gameObject)
        {
            m_Data = GetValue<MiddleBossData>("Data");
            owner = GetValue<MiddleBossController>("Owner");
            base.Setup(gameObject);
        }
    }
}
