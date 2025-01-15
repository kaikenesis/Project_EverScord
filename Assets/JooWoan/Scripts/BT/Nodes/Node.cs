using System.Collections.Generic;
using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class Node
    {
        public enum NodeState
        {
            RUNNING,
            SUCCESS,
            FAILURE
        }

        protected Node parent = null;
        protected List<Node> children = new List<Node>();
        protected NodeState state = NodeState.FAILURE;
        private IDictionary<string, IBlackboard> blackBoard;

        public Node() { }

        public Node(List<Node> children)
        {
            for (int i = 0; i < children.Count; i++)
                Attach(children[i]);
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public void CreateBlackboard()
        {
            blackBoard = new Dictionary<string, IBlackboard>();
        }

        public virtual NodeState Evaluate()
        {
            return state;
        }

        public void SetData(string key, IBlackboard value)
        {
            if (parent != null)
            {
                parent.SetData(key, value);
                return;
            }

            blackBoard[key] = value;
        }

        protected IBlackboard GetData(string key)
        {
            if (parent != null)
                return parent.GetData(key);

            if (!blackBoard.ContainsKey(key))
                return null;

            return blackBoard[key];
        }

        protected virtual bool ClearData(string key)
        {
            if (parent != null)
                return parent.ClearData(key);

            if (!blackBoard.ContainsKey(key))
                return false;

            blackBoard.Remove(key);
            return true;
        }

        protected static bool RollDice()
        {
            return 3 <= Random.Range(1, 7);
        }
    }
}
