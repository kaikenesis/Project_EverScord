using UnityEngine;

public abstract class BTree : MonoBehaviour
{
    [SerializeField] protected BehaviorNode root = null;

    protected void Awake()
    {
        SetupTree();
    }

    private void Update()
    {
        if (root != null)
            root.Evaluate();
    }

    protected abstract void SetupTree();
}
