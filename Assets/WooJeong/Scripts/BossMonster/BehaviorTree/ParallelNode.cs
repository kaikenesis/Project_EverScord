using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelNode : MonoBehaviour, ICompositeNode
{
    public List<INode> ChildList { get; set; } = new();

    public bool Run()
    {
        foreach (var child in ChildList)
        {
            child.Run();
        }
        return true;
    }
}
