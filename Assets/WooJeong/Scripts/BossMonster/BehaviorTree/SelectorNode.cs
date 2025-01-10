using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : MonoBehaviour, ICompositeNode
{
    public List<INode> ChildList { get; set; } = new();

    public bool Run()
    {
        foreach (var child in ChildList)
        {
            if (child.Run())
            {
                return true;
            }
        }
        return false;
    }
}
