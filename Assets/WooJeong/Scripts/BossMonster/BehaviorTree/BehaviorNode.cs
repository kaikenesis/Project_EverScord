using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}

public abstract class BehaviorNode : ScriptableObject
{
    public BehaviorNode parent = null;
    [SerializeField] public List<BehaviorNode> children = new();
    protected NodeState state = NodeState.FAILURE;
    private Dictionary<string, object> blackBoard;
    protected int start = 0;

    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;

    public void Init()
    {
        if (children == null)
            return;
        start = 0;
        int count = children.Count;
        for (int i = 0; i < count; i++)
        {
            children[i].parent = this;
            children[i].Init();
        }
    }

    public virtual void Setup(GameObject gameObject)
    {
        if (children == null)
            return;

        int count = children.Count;
        for (int i = 0; i < count; i++)
        {
            children[i].Setup(gameObject);
        }
    }

    public abstract NodeState Evaluate();

    public void CreateBlackboard()
    {
        blackBoard = new();
    }

    public void SetValue(string key, object value)
    {
        if (parent != null)
        {
            parent.SetValue(key, value);
        }
        else
        {
            blackBoard.Add(key, value);
        }
    }

    protected T GetValue<T>(string key) where T : class
    {
        if (parent != null)
            return parent.GetValue<T>(key);

        if (!blackBoard.ContainsKey(key))
            return null;

        return blackBoard[key] as T;
    }

    protected bool DeleteData(string key)
    {
        if (parent != null)
            return parent.DeleteData(key);

        if (!blackBoard.ContainsKey(key))
            return false;

        blackBoard.Remove(key);
        return true;
    }
}
