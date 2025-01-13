using UnityEngine;

namespace EverScord.BehaviorTree
{
    public abstract class ActTree : MonoBehaviour
    {
        private Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null)
                root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}

