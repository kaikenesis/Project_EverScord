using Photon.Pun;
using UnityEngine;

public abstract class BTree : MonoBehaviour
{
    [SerializeField] protected BehaviorNode root = null;

    protected void Awake()
    {
        SetupTree();
    }

    protected virtual void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (root != null)
            root.Evaluate();
    }

    protected abstract void SetupTree();
}
